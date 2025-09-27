using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    #region Referencias
    private UIManager uiManager;
    private CharacterSpawn characterSpawn;
    private SpriteRenderer spriteRendererPersonaje;
    public PersonasSentadas personasSentadas;
    #endregion

    #region Estado
    public CharacterAttributes personajeActual;
    public int nivelActual = 1;
    public enum ResultadoRecomendacion { Ninguna, Buena, Mala }
    public ResultadoRecomendacion resultadoRecomendacion = ResultadoRecomendacion.Ninguna;
    public int recomendacionesBuenas = 0;
    public int recomendacionesMalas = 0;
    private bool primerClienteDetectado = false;
    #endregion

    #region UI
    public GameObject panelInfoLibro;
    public GameObject panelFinNivel;
    public TMP_Text textoDia;
    public TMP_Text textoResultadoFinal;
    public TMP_Text textoTituloFinDeDia;

    [Header("Localización")]
    public LocalizedString textoDiaLocalized;
    public LocalizedString textoFinDiaLocalized;
    public LocalizedString resumenClientesLocalized;
    public LocalizedString mensajeFinalBienLocalized;
    public LocalizedString mensajeFinalMalLocalized;
    public LocalizedString mensajeFinalRegularLocalized;
    #endregion

    #region Configuración de Niveles
    [System.Serializable]
    public class Nivel
    {
        public GameObject[] personajesDelNivel;
    }

    [Header("Niveles del juego")]
    public Nivel[] niveles;
    #endregion

    #region Ciclo de Vida
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

        if (uiManager == null)
            Debug.LogError("UIManager no encontrado en la escena.");
        if (characterSpawn == null)
            Debug.LogError("CharacterSpawn no encontrado en la escena.");

        SaveData data = SaveManager.CargarNivel();
        nivelActual = Mathf.Max(1, data.nivelActual);
        Invoke("RestaurarEstadoJuego", 0.2f);
        CameraManager.instance?.InicializarCamarasDesdeCarga(nivelActual);
        StartCoroutine(MostrarCartelInicioDia());
    }

    private void RestaurarEstadoJuego()
    {
        SaveData data = SaveManager.CargarNivel();
        SaveManager.RestaurarDatos(data);

        if (TaskManager.instance != null)
        {
            TaskManager.instance.RestaurarEstadoTienda(data.tiendaAbiertaPorPrimeraVez);
        }
    }
    #endregion

    #region Flujo de Día y Niveles
    private IEnumerator MostrarCartelInicioDia()
    {
        if (nivelActual - 1 >= niveles.Length)
        {
            yield break;
        }

        MenuPausa.instance.OcultarBotonPausa();
        InventarioManager.Instance.OcultarInventarioCompleto();
        TaskManager.instance.ReiniciarTareas();
        panelInfoLibro.SetActive(true);

        var handle = textoDiaLocalized.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            string textoLocalizado = handle.Result;
            textoDia.text = string.Format(textoLocalizado, nivelActual);
        }
        else
        {
            textoDia.text = $"Día {nivelActual}";
        }

        if (CameraManager.instance != null)
        {
            CameraManager.instance.botonCambiarCamara3.gameObject.SetActive(nivelActual > 1);
            CameraManager.instance.botonCambiarCamara2.gameObject.SetActive(nivelActual > 2);
            CameraManager.instance.botonCambiarCamara4.gameObject.SetActive(nivelActual > 3);
            CameraManager.instance.botonCambiarCamara5.gameObject.SetActive(nivelActual > 3);
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        panelInfoLibro.SetActive(false);
        Time.timeScale = 1f;
        MenuPausa.instance.MostrarBotonPausa();
        InventarioManager.Instance.MostrarInventarioCompleto();
        TaskManager.instance.InicializarTareasParaNivel();
        FindFirstObjectByType<CatDialogues>().IniciarDialogoDelDia(nivelActual);
        if (Gnomos.instance != null)
            Gnomos.instance.ReiniciarEvento();
    }

    public void FinDeNivel()
    {
        StartCoroutine(MostrarCartelFinDeDia());
    }

    private IEnumerator MostrarCartelFinDeDia()
    {
        TaskManager.instance.OcultarListaTareas();
        MenuPausa.instance.OcultarBotonPausa();
        InventarioManager.Instance.OcultarInventarioCompleto();
        panelFinNivel.gameObject.SetActive(true);

        int diaMostrado = nivelActual;
        nivelActual++;

        var ruidoManager = FindFirstObjectByType<RuidoSalaDeLecturaManager>();
        if (ruidoManager != null)
        {
            ruidoManager.DesactivarSalaRuidosa();
            ruidoManager.CancelarPosibilidadDeEvento();
        }

        var handleTitulo = textoFinDiaLocalized.GetLocalizedStringAsync();
        yield return handleTitulo;

        if (handleTitulo.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            textoTituloFinDeDia.text = string.Format(handleTitulo.Result, diaMostrado);
        }
        else
        {
            textoTituloFinDeDia.text = $"Fin del Día {diaMostrado}";
        }

        var handleResumen = resumenClientesLocalized.GetLocalizedStringAsync();
        yield return handleResumen;

        string resumenClientes;
        if (handleResumen.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            resumenClientes = string.Format(handleResumen.Result, recomendacionesBuenas, recomendacionesMalas);
        }
        else
        {
            resumenClientes = $"Clientes satisfechos: {recomendacionesBuenas}\nClientes insatisfechos: {recomendacionesMalas}";
        }

        LocalizedString mensajeFinalLocalized;
        if (recomendacionesBuenas > recomendacionesMalas)
            mensajeFinalLocalized = mensajeFinalBienLocalized;
        else if (recomendacionesMalas > recomendacionesBuenas)
            mensajeFinalLocalized = mensajeFinalMalLocalized;
        else
            mensajeFinalLocalized = mensajeFinalRegularLocalized;

        var handleMensaje = mensajeFinalLocalized.GetLocalizedStringAsync();
        yield return handleMensaje;

        string mensajeFinal;
        if (handleMensaje.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            mensajeFinal = handleMensaje.Result + "\n" + resumenClientes;
        }
        else
        {
            mensajeFinal = "Resultado del día.\n" + resumenClientes;
        }

        textoResultadoFinal.text = mensajeFinal;
        personasSentadas.DesactivarPersonasSentadas();
        recomendacionesBuenas = 0;
        recomendacionesMalas = 0;
    }

    public void AvanzarAlSiguienteNivel()
    {
        panelFinNivel.SetActive(false);

        if (nivelActual - 1 >= niveles.Length)
        {
            SceneManager.LoadScene("EscenaFinal");
            return;
        }
            SaveManager.GuardarNivel(nivelActual, HistorialManager.Instance.GetLibrosPrestados());


        if (CameraManager.instance != null)
            CameraManager.instance.ActivarCamaraPrincipal();

        if (CobwebManager.instance != null)
            CobwebManager.instance.ReiniciarTelarañas();

        if (TendCat.instance != null)
            TendCat.instance.ReiniciarEstado();

        if (PlantManager.instance != null)
            PlantManager.instance.ReiniciarEstado();

        StartCoroutine(MostrarCartelInicioDia());
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        nivelActual = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }
    #endregion

    #region Personajes

    public bool EsPrimerCliente()
    {
        if (!primerClienteDetectado)
        {
            primerClienteDetectado = true;
            return true;
        }
        return false;
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

        if (nivelActual >= 3)
        {
            personasSentadas.ActivarPersonasSentadas();
        }
    }

    public void EstablecerPersonajeActual(CharacterAttributes personaje)
    {
        personajeActual = personaje;
        spriteRendererPersonaje = personajeActual.GetComponent<SpriteRenderer>();
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
    #endregion

    #region Resultado recomendación 
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
            EconomyManager.instance.SumarDinero(20);

            ShelfManager.instance.RestarLibroEsperadoPorGenero(libro.tipoLibro);
            libro.gameObject.SetActive(false);
            if (CharacterManager.instance.UltimoPersonajeAtendido != null)
            {
                CharacterManager.instance.UltimoPersonajeAtendido.tituloLibroPrestado = libro.titulo;
            }

            AudioManager.instance.sonidoLibroCorrecto.Play();
            ActualizarSpritePersonaje();
            FindFirstObjectByType<HistorialManager>()?.ActualizarLibrosPrestados();
        }
        else
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;

            AudioManager.instance.sonidoLibroIncorrecto.Play();
            ActualizarSpritePersonaje();
        }
    }
    #endregion

    #region Resultado pedidos especiales
    public void CompletarRestauracion()
    {
        resultadoRecomendacion = ResultadoRecomendacion.Buena;
        recomendacionesBuenas++;
        EconomyManager.instance.SumarDinero(30);
        ActualizarSpritePersonaje();
    }
    
    public void CompletarPortada(List<StickerID> stickersUsados, int extras = 0, int cantidadVisibles = 0)
    {
        if (personajeActual == null)
        {
            Debug.LogError("No hay personaje actual asignado para comparar stickers.");
            return;
        }

        if (cantidadVisibles > 6)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;
            Debug.LogWarning("Portada rechazada: se usaron más de 6 stickers visibles.");
            ActualizarSpritePersonaje();
            characterSpawn?.EndInteraction();
            return;
        }

        List<StickerID> stickersRequeridos = personajeActual.stickersRequeridos;
        int totalRequeridos = stickersRequeridos.Count;

        if (totalRequeridos == 0)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Ninguna;
            return;
        }

        int correctos = 0;
        foreach (StickerID requerido in stickersRequeridos)
        {
            if (stickersUsados.Contains(requerido))
                correctos++;
        }

        float porcentaje = (float)correctos / totalRequeridos;

        if (porcentaje < 0.5f)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;
        }
        else if (porcentaje < 1f)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;
            EconomyManager.instance.SumarDinero(15 + extras);
            AudioManager.instance.sonidoEstrellas.Play();
        }
        else
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;
            EconomyManager.instance.SumarDinero(30 + extras);
            AudioManager.instance.sonidoEstrellas.Play();
        }

        ActualizarSpritePersonaje();
        characterSpawn?.EndInteraction();
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
            EconomyManager.instance.SumarDinero(30);
            Debug.Log($"Hechizo completado correctamente: {hechizoRealizado}");
            ActualizarSpritePersonaje();
            AudioManager.instance.sonidoEstrellas.Play();
        }
        else
        {
            resultadoRecomendacion = ResultadoRecomendacion.Mala;
            recomendacionesMalas++;
            EconomyManager.instance.SumarDinero(15);
            Debug.LogWarning($"Hechizo incorrecto. Realizado: {hechizoRealizado}, Solicitado: {personajeActual.hechizoSolicitado}");
            ActualizarSpritePersonaje();
        }
        CameraManager.instance.DesctivarPanelHechizo();
        if (characterSpawn != null)
        {
            characterSpawn.EndInteraction();
        }
    }
    
    public void CompletarTraduccion()
    {
        resultadoRecomendacion = ResultadoRecomendacion.Buena;
        recomendacionesBuenas++;
        EconomyManager.instance.SumarDinero(30);
        ActualizarSpritePersonaje();
    }

    public void CompletarTrivia(int correctas, int incorrectas)
    {
        if (correctas > incorrectas)
        {
            resultadoRecomendacion = ResultadoRecomendacion.Buena;
            recomendacionesBuenas++;

            int recompensa = correctas * 6;
            EconomyManager.instance.SumarDinero(recompensa);

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

    public void ActualizarSpritePorRespuesta(bool fueCorrecta)
    {
        if (spriteRendererPersonaje == null || personajeActual == null)
            return;

        if (fueCorrecta)
        {
            if (personajeActual.spriteRespuestaBuena != null)
                spriteRendererPersonaje.sprite = personajeActual.spriteRespuestaBuena;
        }
        else
        {
            if (personajeActual.spriteRespuestaMala != null)
                spriteRendererPersonaje.sprite = personajeActual.spriteRespuestaMala;
        }
    }
    #endregion

    #region Libros devueltos y donados
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
            EconomyManager.instance.SumarDinero(5);
        }
    }
    #endregion
}