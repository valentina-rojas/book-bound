using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    #region Referencias UI
    public GameObject panelInfoLibro;
    public TMP_Text tituloTexto;
    public TMP_Text descripcionTexto;
    public Image imagenLibroUI;

    public GameObject panelConfirmarSeleccion;
    public Image imagenConfirmarSeleccion;
    public TMP_Text tituloConfirmarSeleccion;

    public Button botonConfirmar;
    public Button botonSiguiente;
    public Button botonAnterior;
    #endregion

    #region Estado interno
    private BookData libroActual;
    private List<BookData> librosMismaSeccion = new List<BookData>();
    private int indiceLibroActual = 0;
    private CharacterSpawn characterSpawn;
    #endregion

    #region Inicialización
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        characterSpawn = FindObjectOfType<CharacterSpawn>();
        if (characterSpawn == null)
            Debug.LogError("CharacterSpawn no encontrado por BookManager.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panelInfoLibro.activeSelf)
            CancelarSeleccion();
    }
    #endregion

    #region Lógica principal
    public void MostrarInformacion(BookData libro)
    {
        libroActual = libro;

        librosMismaSeccion.Clear();
        BookData[] todosLosLibros = FindObjectsOfType<BookData>();

        foreach (BookData b in todosLosLibros)
        {
            if (!b.gameObject.activeInHierarchy || b.tipoLibro != libro.tipoLibro)
                continue;

            Transform parentEstante = b.transform.parent;
            ShelfSlots estante = parentEstante != null ? parentEstante.GetComponent<ShelfSlots>() : null;

            if (estante != null && estante.generoPermitido == b.tipoLibro)
            {
                librosMismaSeccion.Add(b);
            }
        }

        librosMismaSeccion = librosMismaSeccion.OrderBy(b => b.transform.position.x).ToList();
        indiceLibroActual = librosMismaSeccion.IndexOf(libro);

        if (indiceLibroActual == -1)
        {
            libroActual = libro;
            panelInfoLibro.SetActive(true);

            StartCoroutine(libroActual.GetTituloLocalized(titulo => tituloTexto.text = titulo));
            StartCoroutine(libroActual.GetDescripcionLocalized(desc => descripcionTexto.text = desc));
            imagenLibroUI.sprite = libroActual.imagenLibro;

            botonAnterior.interactable = false;
            botonSiguiente.interactable = false;
        }
        else
        {
            MostrarLibroPorIndice(indiceLibroActual);
        }
    }

    private void MostrarLibroPorIndice(int indice)
    {
        if (indice < 0 || indice >= librosMismaSeccion.Count) return;

        libroActual = librosMismaSeccion[indice];
        panelInfoLibro.SetActive(true);

        StartCoroutine(libroActual.GetTituloLocalized(titulo => tituloTexto.text = titulo));
        StartCoroutine(libroActual.GetDescripcionLocalized(desc => descripcionTexto.text = desc));
        imagenLibroUI.sprite = libroActual.imagenLibro;

        botonAnterior.interactable = indice > 0;
        botonSiguiente.interactable = indice < librosMismaSeccion.Count - 1;
    }
    #endregion

    #region Navegación
    public void VerSiguienteLibro()
    {
        if (indiceLibroActual < librosMismaSeccion.Count - 1)
        {
            indiceLibroActual++;
            MostrarLibroPorIndice(indiceLibroActual);
        }
    }

    public void VerLibroAnterior()
    {
        if (indiceLibroActual > 0)
        {
            indiceLibroActual--;
            MostrarLibroPorIndice(indiceLibroActual);
        }
    }
    #endregion

    #region Confirmación y recomendación
    public void ConfirmarSeleccion()
    {
        if (libroActual == null)
        {
            Debug.LogError("No hay libro seleccionado.");
            return;
        }

        Debug.Log("Libro seleccionado: " + libroActual.titulo);
        panelInfoLibro.SetActive(false);
        TaskManager.instance.OcultarListaTareas();
        panelConfirmarSeleccion.SetActive(true);
        imagenConfirmarSeleccion.sprite = libroActual.imagenLibro;

        StartCoroutine(libroActual.GetTituloLocalized(titulo => tituloConfirmarSeleccion.text = titulo));
    }

    public void RecomendarLibro()
    {
        panelConfirmarSeleccion.SetActive(false);

        GameManager.instance.VerificarRecomendacion(libroActual);

        if (characterSpawn != null)
            characterSpawn.EndInteraction();
    }
    #endregion

    #region Botones y control UI
    public void HabilitarBotonConfirmacion()
    {
        botonConfirmar.gameObject.SetActive(true);
    }

    public void DeshabilitarBotonConfirmacion()
    {
        botonConfirmar.gameObject.SetActive(false);
    }

    public void CancelarSeleccion()
    {
        panelInfoLibro.SetActive(false);
    }
    #endregion
}