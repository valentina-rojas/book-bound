using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    [Header("Elementos del tutorial")]
    public GameObject flechaTelaraña;
    public CobwebCleaning telaranaTutorial;
    public GameObject flechaBiblioteca;
    public GameObject flechaVolver;

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
        new string[] { "Tuto12", "Tuto13" }                      // Paso 7: Cierre tutorial
    };

    private void Awake()
    {
        instance = this;
        cat = FindObjectOfType<CatDialogues>();

        if (cat != null)
        {
            cat.OnDialogoExtraFinalizado += OnDialogoExtraFinalizado;
            cat.OnDialogoUltimaLineaTipeada += OnDialogoUltimaLineaTipeada;
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
    private void OnDialogoUltimaLineaTipeada()
    {
        if (pasoActual == 0)
        {
            Debug.Log("Última línea tipeada detectada, iniciando coroutine para habilitar telaraña con delay...");
            StartCoroutine(HabilitarTelaranaConDelay(2f));
        }
    }

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
            else
            {
                if (pasoActual == 2 && flechaVolver != null)
                {
                    Debug.Log("Activando flechaVolver");
                    flechaVolver.SetActive(true);
                    CameraManager.instance.ActivarBotonCamaraTuto();
                }
            }

            if (pasoActual == 0)
            {
                flechaTelaraña?.SetActive(true);
                StartCoroutine(HabilitarTelaranaConDelay(5f));
            }
            else if (pasoActual == 1)
            {
                flechaBiblioteca?.SetActive(false);
            }
        }
    }

    private IEnumerator HabilitarTelaranaConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (telaranaTutorial != null)
        {
            Debug.Log("Habilitando interacción de la telaraña tras delay.");
            telaranaTutorial.HabilitarInteraccion();
        }
        else
        {
            Debug.LogWarning("Referencia a la telaraña del tutorial no asignada.");
        }
    }

    private void OnDialogoExtraFinalizado()
    {
        if (pasoActual == 1)
        {
            TaskManager.instance?.OcultarListaTareas();
            CameraManager.instance.ActivarBotonCamara();
            flechaBiblioteca?.SetActive(true);
        }
        else if (pasoActual == 3)
        {
            Debug.Log("Finalizó Tuto5, mostrando tareas y habilitando botón de tienda.");
            
            if (GameManager.instance != null && GameManager.instance.nivelActual == 1)
            {
                TaskManager.instance.MostrarTareas();
                TaskManager.instance.botonAbrirTienda.gameObject.SetActive(true);
            }
        }
        else if (pasoActual == 5)
        {
            Debug.Log("Finalizó el diálogo Tuto9. Abriendo historial...");
            HistorialManager historial = FindObjectOfType<HistorialManager>();
            if (historial != null)
            {
                historial.AbrirTodo();
                esperandoCierreHistorial = true;
            }
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
            Debug.Log("Primer cliente terminó diálogo inicial. Avanzando al paso 5 del tutorial (Tuto8).");
            AvanzarAlSiguientePaso(); 
        }
        else if (pasoActual == 6)
        {
            Debug.Log("Primer diálogo de resultado finalizado. Avanzando al paso 7 del tutorial (Tuto11 y Tuto12).");
            AvanzarAlSiguientePaso(); 
        }
    }
    
    public void AlCerrarHistorial()
    {
        if (esperandoCierreHistorial && pasoActual == 5)
        {
            Debug.Log("Historial cerrado. Avanzando al paso 6 del tutorial (Tuto10).");
            esperandoCierreHistorial = false;
            AvanzarAlSiguientePaso();
        }
    }
}