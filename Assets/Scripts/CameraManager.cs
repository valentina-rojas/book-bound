using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    #region Variables
    public Camera[] cameras;
    public GameObject[] canvasObjects;
    public int currentCameraIndex = 0;
    public int CurrentCameraIndex => currentCameraIndex;

    [Header("Botones de cámara")]
    public Button botonCambiarCamara0;
    public Button botonCambiarCamara1;
    public Button botonCambiarCamara2;
    public Button botonCambiarCamara3;
    public Button botonCambiarCamara4;

    [Header("Paneles")]
    public GameObject panelReparacion;
    public GameObject panelPortada;
    public GameObject panelHechizo;
    public GameObject panelDonar;
    public GameObject panelDevolver;

    [Header("Managers")]
    public BookCoverManager bookCoverManager;
    #endregion

    #region Unity Lifecycle
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
    #endregion

    #region Cambio de Cámaras
    public void CambiarCamara(int cameraIndex)
    {
        TaskManager.instance.OcultarListaTareas();

        if (cameraIndex < 0 || cameraIndex >= cameras.Length) return;

        cameras[currentCameraIndex].enabled = false;
        if (canvasObjects != null && currentCameraIndex < canvasObjects.Length)
            canvasObjects[currentCameraIndex].SetActive(false);

        currentCameraIndex = cameraIndex;
        cameras[currentCameraIndex].enabled = true;
        if (canvasObjects != null && currentCameraIndex < canvasObjects.Length)
            canvasObjects[currentCameraIndex].SetActive(true);

        Gnomos.instance?.OnCameraChanged(currentCameraIndex);
        
        if (cameraIndex == 1)
            ShelfManager.instance?.RevisarOrganizacion();

        if (cameraIndex == 0 && Tutorial.instance != null)
            Tutorial.instance.AlVolverACamaraPrincipal();
    }

    private System.Collections.IEnumerator VerificarEstantesDespuesDeFrame()
    {
        yield return null;
        ShelfEstante[] estantes = Object.FindObjectsByType<ShelfEstante>(FindObjectsSortMode.None);

        foreach (var estante in estantes)
            estante.VerificarEstante();
    }
    #endregion

    #region Botones de Cámara
    public void DesactivarBotonCamara()
    {
        botonCambiarCamara0.interactable = false;
        botonCambiarCamara1.interactable = false;
        botonCambiarCamara2.interactable = false;
        botonCambiarCamara3.interactable = false;
        botonCambiarCamara4.interactable = false;
    }

    public void ActivarBotonCamara()
    {
        botonCambiarCamara1.interactable = true;
        botonCambiarCamara2.interactable = true;
        botonCambiarCamara3.interactable = true;
        botonCambiarCamara4.interactable = true;
    }

    public void ActivarBotonCamaraTuto()
    {
        botonCambiarCamara0.interactable = true;
    }

    public void ActivarCamaraPrincipal()
    {
        CambiarCamara(0);
    }
    #endregion

    #region Paneles
    public void ActivarPanelReparacion()
    {
        panelReparacion.gameObject.SetActive(true);
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        TaskManager.instance.OcultarBotonTareas();
        PagesManager.instance.ResetSistema();
        PagesManager.instance.DebugPaginas();
        PagesManager.instance.ActivarCategoriaCorrecta();
        PagesManager.instance.DebugPaginas();
    }

    public void DesactivarPanelReparacion()
    {
        panelReparacion.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
    }

    public void ActivarPanelPortada()
    {
        panelPortada.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();

        if (bookCoverManager != null)
            StartCoroutine(bookCoverManager.ActualizarTituloLibroDespuesDeFrame());

        bookCoverManager?.ActivarStickersPorSet();
    }

    public void DesctivarPanelPortada()
    {
        panelPortada.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
    }

    public void ActivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
    }

    public void DesctivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
    }

    public void ActivarPanelDonar()
    {
        DonationManager.instance.ActualizarPortada();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        panelDonar.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareas();
    }

    public void DesctivarPanelDonar()
    {
        panelDonar.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
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
    #endregion
}