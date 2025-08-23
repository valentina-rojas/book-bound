using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public Camera[] cameras;
    public GameObject[] canvasObjects;
    private int currentCameraIndex = 0;
    private bool verificacionInicialHecha = false;
    public Button botonCambiarCamara0;
    public Button botonCambiarCamara1;
    public Button botonCambiarCamara2;
    public Button botonCambiarCamara3;

    public GameObject panelReparacion;
    public GameObject panelPortada;
    public BookCoverManager bookCoverManager;
    public GameObject panelHechizo;
    public GameObject panelDonar;
    public GameObject panelDevolver;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool isActive = (i == 0);
            cameras[i].enabled = isActive;
            if (canvasObjects != null && i < canvasObjects.Length)
                canvasObjects[i].SetActive(isActive);
        }

        int nivel = GameManager.instance.nivelActual;

        botonCambiarCamara2.gameObject.SetActive(nivel > 2);
        botonCambiarCamara3.gameObject.SetActive(nivel > 1);
        botonCambiarCamara0.interactable = false;
    }

    public void CambiarCamara(int cameraIndex)
    {
        TaskManager.instance.OcultarListaTareas();

        if (cameraIndex < 0 || cameraIndex >= cameras.Length)
        {
            return;
        }

        cameras[currentCameraIndex].enabled = false;
        if (canvasObjects != null && currentCameraIndex < canvasObjects.Length)
            canvasObjects[currentCameraIndex].SetActive(false);

        currentCameraIndex = cameraIndex;

        cameras[currentCameraIndex].enabled = true;
        if (canvasObjects != null && currentCameraIndex < canvasObjects.Length)
            canvasObjects[currentCameraIndex].SetActive(true);

        if (cameraIndex == 1)
        {
            ShelfManager.instance?.IntentarDesorganizarLibros();

            if (!verificacionInicialHecha)
            {
                StartCoroutine(VerificarEstantesDespuesDeFrame());
                verificacionInicialHecha = true;
            }
        }

        if (cameraIndex == 0 && Tutorial.instance != null)
        {
            Tutorial.instance.AlVolverACamaraPrincipal();
        }
    }

    private System.Collections.IEnumerator VerificarEstantesDespuesDeFrame()
    {
        yield return null;

        ShelfEstante[] estantes = Object.FindObjectsByType<ShelfEstante>(FindObjectsSortMode.None);
        foreach (var estante in estantes)
        {
            estante.VerificarEstante();
        }
    }

    public void DesactivarBotonCamara()
    {
        botonCambiarCamara0.interactable = false;
        botonCambiarCamara1.interactable = false;
        botonCambiarCamara2.interactable = false;
        botonCambiarCamara3.interactable = false;
    }

    public void ActivarBotonCamara()
    {
        botonCambiarCamara1.interactable = true;
        botonCambiarCamara2.interactable = true;
        botonCambiarCamara3.interactable = true;
    }

    public void ActivarBotonCamaraTuto()
    {
        botonCambiarCamara0.interactable = true;
    }

    public void ActivarPanelReparacion()
    {
        panelReparacion.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
        PagesManager.instance.ResetSistema();
        PagesManager.instance.DebugPaginas();
        PagesManager.instance.ActivarCategoriaCorrecta();
        PagesManager.instance.DebugPaginas();
    }

    public void DesactivarPanelReparacion()
    {
        panelReparacion.gameObject.SetActive(false);
    }

public void ActivarPanelPortada()
{
    panelPortada.SetActive(true);  // primero activamos el panel
    TaskManager.instance.OcultarBotonTareas();

    // iniciamos la actualización del título en runtime
    if (bookCoverManager != null)
        StartCoroutine(bookCoverManager.ActualizarTituloLibroDespuesDeFrame());

    bookCoverManager?.ActivarStickersPorSet();
}


    public void DesctivarPanelPortada()
    {
        panelPortada.gameObject.SetActive(false);
    }

    public void ActivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
    }

    public void DesctivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(false);
    }

    public void ActivarPanelDonar()
    {
        DonationManager.instance.ActualizarPortada();
        panelDonar.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
    }

    public void DesctivarPanelDonar()
    {
        panelDonar.gameObject.SetActive(false);
    }

    public void ActivarPanelDevolver()
    {
        DevolverLibro.instance.MostrarPanelDevolucion();
        panelDevolver.gameObject.SetActive(true);
    }

    public void DesctivarPanelDevolver()
    {
        panelDevolver.gameObject.SetActive(false);
    }

    public void ActivarCamaraPrincipal()
    {
        CambiarCamara(0);
    }
}