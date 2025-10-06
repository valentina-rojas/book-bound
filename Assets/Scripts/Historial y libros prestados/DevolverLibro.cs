using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DevolverLibro : MonoBehaviour
{
    public static DevolverLibro instance;

    [Header("Referencias UI")]
    public GameObject canvasDevolucion;
    public Image imagenLibro;
    public Button botonConfirmarDevolucion;
    public TMP_Text tituloLibroTexto;

    [Header("Sprites por género")]
    public Sprite fantasiaSprite;
    public Sprite misterioSprite;
    public Sprite pocionesSprite;
    public Sprite herbologiaSprite;
    public Sprite recetasSprite;
    public Sprite historiaSprite;
    public Sprite terrorSprite;
    public Sprite spriteDefault;

    private Dictionary<string, Sprite> spritesPorGenero;
    private BookData libroActual;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (botonConfirmarDevolucion != null)
            botonConfirmarDevolucion.gameObject.SetActive(false);

        spritesPorGenero = new Dictionary<string, Sprite>
        {
            { "fantasia", fantasiaSprite },
            { "misterio", misterioSprite },
            { "pociones", pocionesSprite },
            { "herbologia", herbologiaSprite },
            { "recetas", recetasSprite },
            { "historia", historiaSprite },
            { "terror", terrorSprite }
        };

        if (botonConfirmarDevolucion != null)
            botonConfirmarDevolucion.onClick.AddListener(ConfirmarDevolucion);
    }

    public void MostrarPanelDevolucion()
    {
        if (canvasDevolucion != null)
            canvasDevolucion.SetActive(true);

        CharacterAttributes personaje = GameManager.instance.personajeActual;
        if (personaje == null)
        {
            Debug.LogWarning("No hay personaje actual para mostrar devolución.");
            imagenLibro.sprite = spriteDefault;
            if (tituloLibroTexto != null)
                tituloLibroTexto.text = "";
            return;
        }

        int libroID = personaje.libroDevueltoID;
        BookData[] libros = Resources.FindObjectsOfTypeAll<BookData>();

        libroActual = null;
        foreach (BookData libro in libros)
        {
            if (libro.libroID == libroID)
            {
                libroActual = libro;
                break;
            }
        }

        if (libroActual == null)
        {
            Debug.LogWarning($"No se encontró el libro con ID {libroID}.");
            imagenLibro.sprite = spriteDefault;
            if (tituloLibroTexto != null)
                tituloLibroTexto.text = "";
        }
        else
        {
            string genero = libroActual.tipoLibro.ToLower().Trim();
            if (!string.IsNullOrEmpty(genero) && spritesPorGenero.TryGetValue(genero, out Sprite spriteGenero))
            {
                imagenLibro.sprite = spriteGenero;
                Debug.Log($"Portada actualizada con sprite de género: {genero}");
            }
            else
            {
                imagenLibro.sprite = spriteDefault;
                Debug.LogWarning($"No se encontró sprite para el género '{genero}', se usa sprite default.");
            }

            imagenLibro.preserveAspect = true;

            if (tituloLibroTexto != null)
            {
                StartCoroutine(libroActual.GetTituloLocalized(titulo =>
                {
                    tituloLibroTexto.text = string.IsNullOrEmpty(titulo) ? libroActual.titulo : titulo;
                }));
            }
        }

        if (botonConfirmarDevolucion != null)
            botonConfirmarDevolucion.gameObject.SetActive(true);
    }

    public void ConfirmarDevolucion()
    {
        if (libroActual == null)
        {
            Debug.LogWarning("No hay libro seleccionado para devolver.");
            return;
        }

        libroActual.gameObject.SetActive(true);
        Debug.Log($"Libro con ID {libroActual.libroID} activado.");

        HistorialManager historial = FindFirstObjectByType<HistorialManager>();
        if (historial != null)
        {
            historial.RegistrarDevolucion(libroActual.titulo);
            historial.StartCoroutine(historial.MostrarLibrosPrestados());
        }

        string genero = libroActual.tipoLibro.ToLower().Trim();
        if (!string.IsNullOrEmpty(genero))
        {
            ShelfManager.instance.SumarLibroEsperadoPorGenero(genero);
            Debug.Log($"Sumado libro devuelto al género: {genero}");
        }

        CameraManager.instance.DesctivarPanelDevolver();
        GameManager.instance.LibroDevuelto();

        if (canvasDevolucion != null)
            canvasDevolucion.SetActive(false);
    }
}