using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    #region Elementos del Tutorial
    public GameObject flechaTelaraña;
    public CobwebCleaning telaranaTutorial;
    public GameObject flechaBiblioteca;
    public GameObject flechaVolver;
    #endregion

    #region Variables Privadas
    private CatDialogues cat;
    private bool esperandoCierreHistorial = false;
    private int pasoActual = 0;

    private string[][] dialogosPorPaso = new string[][]
    {
        new string[] { "Tuto1" },                                // Paso 0: Introducción + activar telaraña
        new string[] { "Tuto2", "Tuto3", "Tuto4" },              // Paso 1: Explicación limpieza telaraña + tareas
        new string[] { },                                        // Paso 2: Espera acción del jugador (volver a cámara)
        new string[] { "Tuto5" },                                // Paso 3: Mostrar tareas y botón tienda
        new string[] { "Tuto6", "Tuto7", "Tuto8" },              // Paso 4: Diálogo con primer cliente
        new string[] { "Tuto9", "Tuto10" },                      // Paso 5: Abrir historial tras diálogo
        new string[] { "Tuto11" },                               // Paso 6: Resultado primer cliente
        new string[] { "Tuto12", "Tuto13", "Tuto14" }           // Paso 7: Cierre tutorial
    };
    #endregion

    #region Ciclo de Vida
    private void Awake()
    {
        instance = this;
        cat = Object.FindFirstObjectByType<CatDialogues>();

        if (cat != null)
        {
            cat.OnDialogoExtraFinalizado += OnDialogoExtraFinalizado;
            cat.OnDialogoUltimaLineaTipeada += OnDialogoUltimaLineaTipeada;
        }

        SaveData saveData = SaveManager.CargarNivel();
        if (saveData.nivelActual > 1)
        {
            if (telaranaTutorial != null)
                telaranaTutorial.HabilitarInteraccion();

            if (TaskManager.instance != null)
            {
                TaskManager.instance.MostrarTareas();
                TaskManager.instance.botonAbrirTienda.gameObject.SetActive(true);
            }

            CameraManager.instance?.ActivarCamaraPrincipal();
            CameraManager.instance?.ActivarBotonCamara();

            flechaTelaraña?.SetActive(false);
            flechaBiblioteca?.SetActive(false);
            flechaVolver?.SetActive(false);

            enabled = false; 
        }
    }

    private void OnDestroy()
    {
        if (cat != null)
        {
            cat.OnDialogoExtraFinalizado -= OnDialogoExtraFinalizado;
            cat.OnDialogoUltimaLineaTipeada -= OnDialogoUltimaLineaTipeada;
        }
    }
    #endregion

    #region Eventos del Cat
    private void OnDialogoUltimaLineaTipeada()
    {
        if (pasoActual == 0 && GameManager.instance.nivelActual == 1)
        {
            StartCoroutine(HabilitarTelaranaConDelay(2f));
        }
    }

    private void OnDialogoExtraFinalizado()
    {
        if (pasoActual == 1 && GameManager.instance.nivelActual == 1)
        {
            TaskManager.instance?.OcultarListaTareas();
            CameraManager.instance.ActivarBotonCamara();
            flechaBiblioteca?.SetActive(true);
        }
        else if (pasoActual == 3 && GameManager.instance.nivelActual == 1)
        {
            TaskManager.instance.MostrarTareas();
            TaskManager.instance.botonAbrirTienda.gameObject.SetActive(true);
        }
        else if (pasoActual == 5)
        {
            HistorialManager historial = Object.FindFirstObjectByType<HistorialManager>();
            if (historial != null)
            {
                historial.AbrirTodo();
                esperandoCierreHistorial = true;
            }
        }
    }
    #endregion

    #region Tutorial Control
    public void EmpezarTutorial()
    {
        pasoActual = 0;
        MostrarPasoActual();
    }

    private void MostrarPasoActual()
    {
        if (cat == null) return;

        flechaTelaraña?.SetActive(false);
        flechaBiblioteca?.SetActive(false);
        flechaVolver?.SetActive(false);

        if (pasoActual < dialogosPorPaso.Length)
        {
            string[] dialogos = dialogosPorPaso[pasoActual];
            if (dialogos.Length > 0)
            {
                cat.IniciarDialogoExtraDesdeLista(dialogos);
            }
            else if (pasoActual == 2 && GameManager.instance.nivelActual == 1)
            {
                flechaVolver.SetActive(true);
            }

            if (pasoActual == 0 && GameManager.instance.nivelActual == 1)
            {
                flechaTelaraña?.SetActive(true);
                StartCoroutine(HabilitarTelaranaConDelay(2f));
            }
        }
    }

    private IEnumerator HabilitarTelaranaConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (telaranaTutorial != null)
        {
            telaranaTutorial.HabilitarInteraccion();
        }
    }

    public void AvanzarAlSiguientePaso()
    {
        pasoActual++;
        MostrarPasoActual();
    }

    public void MostrarFlechaVolver()
    {
        pasoActual = 2;
        MostrarPasoActual();
    }

    public void AlVolverACamaraPrincipal()
    {
        if (pasoActual == 2)
        {
            pasoActual = 3;
            MostrarPasoActual();
        }
    }

    public void PrimerClienteTerminoDialogo()
    {
        if (pasoActual == 4)
        {
            AvanzarAlSiguientePaso();
        }
        else if (pasoActual == 6)
        {
            AvanzarAlSiguientePaso();
        }
    }

    public void AlCerrarHistorial()
    {
        if (esperandoCierreHistorial && pasoActual == 5)
        {
            esperandoCierreHistorial = false;
            AvanzarAlSiguientePaso();
        }
    }

    public void SaltarTutorial()
    {
        pasoActual = dialogosPorPaso.Length;

        flechaTelaraña?.SetActive(false);
        flechaBiblioteca?.SetActive(false);
        flechaVolver?.SetActive(false);

        if (telaranaTutorial != null)
            telaranaTutorial.HabilitarInteraccion();

        CameraManager.instance?.ActivarCamaraPrincipal();
        CameraManager.instance?.ActivarBotonCamara();
        TaskManager.instance?.MostrarTareas();
        if (GameManager.instance != null)
            TaskManager.instance.botonAbrirTienda.gameObject.SetActive(true);

        enabled = false;
    }
    #endregion
}