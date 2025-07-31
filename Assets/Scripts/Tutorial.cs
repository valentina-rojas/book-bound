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
    private int pasoActual = 0;

    private string[][] dialogosPorPaso = new string[][]
    {
        new string[] { "Tuto1" },
        new string[] { "Tuto2", "Tuto3", "Tuto4" },
        new string[] { },
        new string[] { "Tuto5" },
        new string[] { "Tuto6", "Tuto7" }                        
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
            else if (pasoActual == 3)
            {
                if (GameManager.instance != null && GameManager.instance.nivelActual == 1)
                {
                    TaskManager.instance.MostrarTareas();
                    TaskManager.instance.botonAbrirTienda.gameObject.SetActive(true);
                }
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
            flechaBiblioteca?.SetActive(true);
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
}