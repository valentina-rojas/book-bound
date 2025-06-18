using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DonationManager : MonoBehaviour
{
    public static DonationManager instance;

    [Header("Referencias")]
    public GameObject canvasDonacion;
    public Image imagenPortada;
    public Sprite spriteDefault;
    public Button botonAceptarDonacion;  

    [Header("Audio")]
    public AudioClip sonidoAperturaPanel;
    private AudioSource audioSource;

    [Header("Animación")]
    public float duracionAnimacion = 1f; 

    [Header("Sprites por género")]
    public Sprite fantasiaSprite;
    public Sprite misterioSprite;
    public Sprite pocionesSprite;
    public Sprite herbologiaSprite;
    public Sprite recetasSprite;
    public Sprite historiaSprite;
    public Sprite terrorSprite;

    private Dictionary<string, Sprite> spritesPorGenero;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

        if (botonAceptarDonacion != null)
            botonAceptarDonacion.gameObject.SetActive(false);
    }

    public void ActualizarPortada()
    {
        if (canvasDonacion != null)
            canvasDonacion.SetActive(true);

        if (imagenPortada == null)
        {
            Debug.LogWarning("imagenPortada no está asignada.");
            return;
        }

        CharacterAttributes personaje = GameManager.instance.personajeActual;
        if (personaje == null)
        {
            Debug.LogWarning("No hay personaje actual asignado.");
            imagenPortada.sprite = spriteDefault;
            return;
        }

        int libroID = personaje.libroDonadoID;
        BookData[] libros = Resources.FindObjectsOfTypeAll<BookData>();

        foreach (BookData libro in libros)
        {
            if (libro.libroID == libroID)
            {
                string genero = libro.tipoLibro.ToLower().Trim();

                if (!string.IsNullOrEmpty(genero) && spritesPorGenero.TryGetValue(genero, out Sprite spriteGenero))
                {
                    imagenPortada.sprite = spriteGenero;
                    Debug.Log($"Portada actualizada con sprite de género: {genero}");
                }
                else
                {
                    imagenPortada.sprite = spriteDefault;
                    Debug.LogWarning($"No se encontró sprite para el género '{genero}', se usa sprite default.");
                }

                imagenPortada.transform.localScale = Vector3.zero;

                if (botonAceptarDonacion != null)
                    botonAceptarDonacion.gameObject.SetActive(false);

                StartCoroutine(AnimarPortadaYSonido());

                return;
            }
        }

        imagenPortada.sprite = spriteDefault;
        Debug.LogWarning($"Libro con ID {libroID} no encontrado. Portada por defecto asignada.");
    }

    private IEnumerator AnimarPortadaYSonido()
    {
        if (sonidoAperturaPanel != null)
        {
            audioSource.PlayOneShot(sonidoAperturaPanel);
        }

        float tiempo = 0f;
        Vector3 escalaInicial = Vector3.zero;
        Vector3 escalaFinal = Vector3.one;

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionAnimacion;
            imagenPortada.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            yield return null;
        }

        imagenPortada.transform.localScale = escalaFinal;

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        if (botonAceptarDonacion != null)
            botonAceptarDonacion.gameObject.SetActive(true);
    }

    public void AceptarDonacion()
    {
        CharacterAttributes personaje = GameManager.instance.personajeActual;

        if (personaje == null)
        {
            Debug.LogWarning("No hay personaje actual asignado en GameManager.");
            return;
        }

        int libroID = personaje.libroDonadoID;
        Debug.Log($"Buscando libro donado con ID: {libroID}");

        BookData[] libros = Resources.FindObjectsOfTypeAll<BookData>();
        Debug.Log($"Libros encontrados: {libros.Length}");

        foreach (BookData libro in libros)
        {
            if (libro.libroID == libroID)
            {
                libro.gameObject.SetActive(true);
                Debug.Log($"Libro con ID {libroID} activado.");

                string genero = libro.tipoLibro.ToLower().Trim();

                if (!string.IsNullOrEmpty(genero))
                {
                    ShelfManager.instance.SumarLibroEsperadoPorGenero(genero);
                    Debug.Log($"Sumado libro al género: {genero}");
                }

                break;
            }
        }

        CameraManager.instance.DesctivarPanelDonar();
        GameManager.instance.LibroDonado();

        if (canvasDonacion != null)
            canvasDonacion.SetActive(false);
    }
}