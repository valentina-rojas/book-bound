using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using static EventManager;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    #region Referencias UI
    public GameObject panelInfoLibro;
    public TMP_Text tituloTexto;
    public TMP_Text descripcionTexto;
    public Image imagenLibroUI;
    public TMP_Text textoIndicacion;
    public Button botonCamaraMostrador;
    public Button botonCamaraPatio;

    public GameObject panelConfirmarSeleccion;
    public Image imagenConfirmarSeleccion;
    public TMP_Text tituloConfirmarSeleccion;

    public Button botonConfirmar;
    public Button botonDeseleccionar; 
    public GameObject panelOtroLibroSeleccionado;
    public Button botonSiguiente;
    public Button botonAnterior;
    #endregion

    #region Estado interno
    private BookData libroActual;
    private List<BookData> librosMismaSeccion = new List<BookData>();
    private int indiceLibroActual = 0;
    private CharacterSpawn characterSpawn;
    private BookData libroConfirmado;
    #endregion

    #region Inicialización
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        characterSpawn = FindFirstObjectByType<CharacterSpawn>();
        if (characterSpawn == null)
            Debug.LogError("CharacterSpawn no encontrado por BookManager.");

        if (botonDeseleccionar != null)
            botonDeseleccionar.onClick.AddListener(DeseleccionarLibro);

        panelOtroLibroSeleccionado?.SetActive(false); 
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
        TaskManager.instance.OcultarListaTareas();
        libroActual = libro;
        StaticVariables.SessionData.bookOpened = true;
        librosMismaSeccion.Clear();
        BookData[] todosLosLibros = FindObjectsByType<BookData>(FindObjectsSortMode.None);

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
        if (textoIndicacion != null)
        {
            if (GameManager.instance.nivelActual == 1 && !TaskManager.instance.SeAbrioTiendaAlMenosUnaVez())
            {
                textoIndicacion.gameObject.SetActive(true);
            }
            else
            {
                textoIndicacion.gameObject.SetActive(false);
            }
        }
        if (botonCamaraMostrador != null) botonCamaraMostrador.gameObject.SetActive(false);
        if (botonCamaraPatio != null) botonCamaraPatio.gameObject.SetActive(false);

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

        if (libroConfirmado != null)
        {
            if (libroActual == libroConfirmado)
            {
                botonDeseleccionar.gameObject.SetActive(true);
                panelOtroLibroSeleccionado.SetActive(false);
            }
            else
            {
                botonDeseleccionar.gameObject.SetActive(false);
                botonConfirmar.gameObject.SetActive(false);
                panelOtroLibroSeleccionado.SetActive(true);
            }
        }
        else
        {
            botonDeseleccionar.gameObject.SetActive(false);
            panelOtroLibroSeleccionado.SetActive(false);
        }
    }
    #endregion

    #region Navegación
    public void VerSiguienteLibro()
    {
        if (indiceLibroActual < librosMismaSeccion.Count - 1)
        {
            AudioManager.instance.cambioLibro.Play();
            indiceLibroActual++;
            MostrarLibroPorIndice(indiceLibroActual);
        }
    }

    public void VerLibroAnterior()
    {
        if (indiceLibroActual > 0)
        {
            AudioManager.instance.cambioLibro.Play();
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

        libroConfirmado = libroActual; 

        Debug.Log("Libro seleccionado: " + libroActual.titulo);
        TaskManager.instance.OcultarListaTareas();
        panelConfirmarSeleccion.SetActive(true);
        imagenConfirmarSeleccion.sprite = libroActual.imagenLibro;
        StartCoroutine(libroActual.GetTituloLocalized(titulo => tituloConfirmarSeleccion.text = titulo));
        botonDeseleccionar.gameObject.SetActive(true); 

        RegistrarEventoLibroSeleccionado(libroActual);
    }

    private void RegistrarEventoLibroSeleccionado(BookData libro)
    {
        LibrosEvent libroEvent = new LibrosEvent();
        libroEvent.bookId = libro.libroID.ToString();
        libroEvent.opened = true;
        libroEvent.level = GameManager.instance.nivelActual;

        bool selectedCorrectly = false;
        if (GameManager.instance.personajeActual != null)
        {
            selectedCorrectly = GameManager.instance.personajeActual.libroDeseadoID == libro.libroID;
        }

        libroEvent.selectedCorrectly = selectedCorrectly;

#if !UNITY_EDITOR
    AnalyticsService.Instance.RecordEvent(libroEvent);
#else
        Debug.Log($"[ANALYTICS] LibrosEvent: bookId={libro.libroID}, opened=true, selectedCorrectly={selectedCorrectly}, level={GameManager.instance.nivelActual}");
#endif
    }

    public void RecomendarLibro()
    {
        panelConfirmarSeleccion.SetActive(false);

        GameManager.instance.VerificarRecomendacion(libroActual);

        libroConfirmado = null;
        libroActual = null;

        panelInfoLibro.SetActive(false);
        panelOtroLibroSeleccionado.SetActive(false);
        botonDeseleccionar.gameObject.SetActive(false);
        botonConfirmar.gameObject.SetActive(true);

        if (botonCamaraMostrador != null) botonCamaraMostrador.gameObject.SetActive(true);
        if (botonCamaraPatio != null) botonCamaraPatio.gameObject.SetActive(true);

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
        if (botonCamaraMostrador != null) botonCamaraMostrador.gameObject.SetActive(true);
        if (botonCamaraPatio != null) botonCamaraPatio.gameObject.SetActive(true);
    }

    public void DeseleccionarLibro()
    {
        if (libroConfirmado == null)
            return;

        panelConfirmarSeleccion.SetActive(false);
        panelOtroLibroSeleccionado.SetActive(false);

        libroConfirmado = null;

        panelInfoLibro.SetActive(true);

        if (botonCamaraMostrador != null) botonCamaraMostrador.gameObject.SetActive(false);
        if (botonCamaraPatio != null) botonCamaraPatio.gameObject.SetActive(false);

        botonDeseleccionar.gameObject.SetActive(false);
        botonConfirmar.gameObject.SetActive(true);
        Debug.Log("Libro deseleccionado.");
    }
    #endregion
}