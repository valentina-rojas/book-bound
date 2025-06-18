using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UIManager uiManager;
    private CharacterSpawn characterSpawn;
    private SpriteRenderer spriteRendererPersonaje;
    [SerializeField] private GameObject salaLecturaGO; 

    [Header("Estado del juego")]
    public CharacterAttributes personajeActual;

    [Header("Sonidos")]
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    public AudioClip sonidoEstrellas;
    private AudioSource audioSource;

    public GameObject panelInfoLibro;
    public GameObject panelFinNivel;
    public TMP_Text textoDia;
    public int nivelActual = 1;

    public enum ResultadoRecomendacion { Ninguna, Buena, Mala }
    public ResultadoRecomendacion resultadoRecomendacion = ResultadoRecomendacion.Ninguna;

    public int recomendacionesBuenas = 0;
    public int recomendacionesMalas = 0;

    public TMP_Text textoResultadoFinal;
    public TMP_Text textoTituloFinDeDia;

    [System.Serializable]
    public class Nivel
    {
        public GameObject[] personajesDelNivel;
    }

    [Header("Niveles del juego")]
    public Nivel[] niveles;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        characterSpawn = FindFirstObjectByType<CharacterSpawn>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.volume = 0.7f; 
        
        if (uiManager == null)
            Debug.LogError("UIManager no encontrado en la escena.");

        if (characterSpawn == null)
            Debug.LogError("CharacterSpawn no encontrado en la escena.");

        StartCoroutine(MostrarCartelInicioDia());
    }

    private IEnumerator MostrarCartelInicioDia()
    {
        MenuPausa.instance.OcultarBotonPausa();
        TaskManager.instance.ReiniciarTareas();
        panelInfoLibro.SetActive(true);
        textoDia.text = $"Día {nivelActual}";

        if (nivelActual == 2)
            CameraManager.instance.botonCambiarCamara3.gameObject.SetActive(true);
        
        if (nivelActual == 4)
            CameraManager.instance.botonCambiarCamara2.gameObject.SetActive(true);

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        panelInfoLibro.SetActive(false);
        Time.timeScale = 1f;

        MenuPausa.instance.MostrarBotonPausa();
        TaskManager.instance.InicializarTareasParaNivel();
        FindFirstObjectByType<CatDialogues>().IniciarDialogoDelDia(nivelActual);
    }


    public void IniciarSpawnDePersonajes()
    {
        TaskManager.instance.OcultarListaTareas();

        if (nivelActual - 1 < niveles.Length)
        {
            characterSpawn.AsignarPersonajesDelNivel(niveles[nivelActual - 1].personajesDelNivel);
            characterSpawn.ComenzarSpawn();
        }
        else
        {
            Debug.LogWarning("No hay más niveles definidos.");
        }
    }

    public void EstablecerPersonajeActual(CharacterAttributes personaje)
    {
        personajeActual = personaje;
        spriteRendererPersonaje = personajeActual.GetComponent<SpriteRenderer>();
    }


    public void VerificarRecomendacion(BookData libro)
    {
        if (personajeActual == null)
        {
            Debug.LogError("No hay personaje actual asignado.");
            return;
        }

        bool esCorrecto = personajeActual.libroDeseadoID == libro.libroID;
        bool esDelTipoPreferido = personajeActual.tipoPreferido == libro.tipoLibro;

        if (esCorrecto)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;

            ShelfManager.instance.RestarLibroEsperadoPorGenero(libro.tipoLibro);
            libro.gameObject.SetActive(false);
            if (CharacterManager.instance.UltimoPersonajeAtendido != null)
            {
                CharacterManager.instance.UltimoPersonajeAtendido.tituloLibroPrestado = libro.titulo;
            }

            audioSource.PlayOneShot(sonidoCorrecto);
            ActualizarSpritePersonaje();
        }
        else
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;

            audioSource.PlayOneShot(sonidoIncorrecto);
            ActualizarSpritePersonaje();
        }
    }

    public void CompletarRestauracion()
    {
        Debug.Log("Restauración completada.");
        resultadoRecomendacion = ResultadoRecomendacion.Buena;
        recomendacionesBuenas++;
        ActualizarSpritePersonaje();

    }

    public void CompletarPortada(List<StickerID> stickersUsados)
    {
        Debug.Log("Portada completada.");

        if (personajeActual == null)
        {
            Debug.LogError("No hay personaje actual asignado para comparar stickers.");
            return;
        }

        List<StickerID> stickersRequeridos = personajeActual.stickersRequeridos;

        Debug.Log($"Stickers requeridos ({stickersRequeridos.Count}): {string.Join(", ", stickersRequeridos)}");
        Debug.Log($"Stickers usados ({stickersUsados.Count}): {string.Join(", ", stickersUsados)}");

        bool tieneTodos = true;

        foreach (StickerID requerido in stickersRequeridos)
        {
            if (!stickersUsados.Contains(requerido))
            {
                Debug.LogWarning($"Falta sticker requerido: {requerido}");
                tieneTodos = false;
                break;
            }
            else
            {
                Debug.Log($"Sticker requerido presente: {requerido}");
            }
        }

        resultadoRecomendacion = tieneTodos ? ResultadoRecomendacion.Buena : ResultadoRecomendacion.Mala;

        if (tieneTodos)
        {
            recomendacionesBuenas++;
            audioSource.PlayOneShot(sonidoEstrellas);
            ActualizarSpritePersonaje();
        }
        else
            recomendacionesMalas++;
            ActualizarSpritePersonaje();

        Debug.Log("Resultado recomendación: " + resultadoRecomendacion);

        CameraManager.instance.DesctivarPanelPortada();

        if (characterSpawn != null)
        {
            characterSpawn.EndInteraction();
        }
    }

    public void CompletarHechizo(CharacterAttributes.Hechizo hechizoRealizado)
    {
        if (personajeActual == null)
        {
            Debug.LogError("No hay personaje actual asignado.");
            return;
        }
        if (hechizoRealizado == personajeActual.hechizoSolicitado)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;
            Debug.Log($"Hechizo completado correctamente: {hechizoRealizado}");
            ActualizarSpritePersonaje();
            audioSource.PlayOneShot(sonidoEstrellas);
        }
        else
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;
            Debug.LogWarning($"Hechizo incorrecto. Realizado: {hechizoRealizado}, Solicitado: {personajeActual.hechizoSolicitado}");
            ActualizarSpritePersonaje();
        }
        CameraManager.instance.DesctivarPanelHechizo();
        if (characterSpawn != null)
        {
            characterSpawn.EndInteraction();
        }
    }
    
    public void LibroDonado()
    {
        if (characterSpawn != null)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            characterSpawn.EndInteraction();
        }
    }

    public void LibroDevuelto()
    {
        if (characterSpawn != null)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            characterSpawn.EndInteraction();
        }
    }

     public void CompletarTrivia(int correctas, int incorrectas)
    {
        if (correctas > incorrectas)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;
            ActualizarSpritePersonaje();
        }
        else if (incorrectas > correctas)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;
            ActualizarSpritePersonaje();
        }
        if (characterSpawn != null)
        {
            characterSpawn.EndInteraction();
        }
    } 

    private void ActualizarSpritePersonaje()
    {
        if (spriteRendererPersonaje == null || personajeActual == null)
            return;

        switch (resultadoRecomendacion)
        {
            case ResultadoRecomendacion.Buena:
                if (personajeActual.spriteRespuestaBuena != null)
                    spriteRendererPersonaje.sprite = personajeActual.spriteRespuestaBuena;
                break;

            case ResultadoRecomendacion.Mala:
                if (personajeActual.spriteRespuestaMala != null)
                    spriteRendererPersonaje.sprite = personajeActual.spriteRespuestaMala;
                break;

            case ResultadoRecomendacion.Ninguna:
            default:
                break;
        }
    }

    public void FinDeNivel()
    {
        TaskManager.instance.OcultarListaTareas();
        TaskManager.instance.OcultarBotonTareas();
        MenuPausa.instance.OcultarBotonPausa();
        nivelActual++;
        panelFinNivel.gameObject.SetActive(true);
        textoTituloFinDeDia.text = $"Fin del Día {nivelActual - 1}";

        string mensajeFinal = "";

        string resumenClientes = $"Clientes satisfechos: {recomendacionesBuenas}\nClientes insatisfechos: {recomendacionesMalas}\n";

        if (recomendacionesBuenas > recomendacionesMalas)
        {
            mensajeFinal = "¡Buen trabajo! Tus recomendaciones ayudaron a muchos clientes.\n\n" + resumenClientes;
        }
        else if (recomendacionesMalas > recomendacionesBuenas)
        {
            mensajeFinal = "Hoy no fue el mejor día... ¡Seguro mañana será mejor!\n\n" + resumenClientes;
        }
        else
        {
            mensajeFinal = "Un día regular. ¡Seguro mañana será mejor!\n\n" + resumenClientes;
        }

        textoResultadoFinal.text = mensajeFinal;
        recomendacionesBuenas = 0;
        recomendacionesMalas = 0;
    }
    public void AvanzarAlSiguienteNivel()
    {
        panelFinNivel.SetActive(false);
        
        if (ShelfManager.instance != null)
            ShelfManager.instance.ReiniciarEstado();

        if (CameraManager.instance != null)
            CameraManager.instance.ActivarCamaraPrincipal();

        if (CobwebManager.instance != null)
            CobwebManager.instance.ReiniciarTelarañas();
        
        if (TendCat.instance != null) 
            TendCat.instance.ReiniciarEstado();
        
        if (PlantManager.instance != null)
            PlantManager.instance.ReiniciarEstado();
        ShelfManager.instance?.AvanzarContadorDesorden();
        StartCoroutine(MostrarCartelInicioDia());
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        nivelActual = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }
}