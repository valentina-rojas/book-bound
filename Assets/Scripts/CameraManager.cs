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
    public Button botonCambiarCamara5;

    [Header("Paneles")]
    public GameObject panelReparacion;
    public GameObject panelPortada;
    public GameObject panelHechizo;
    public GameObject panelTraduccion; 
    public GameObject panelEncanto;
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

    public void InicializarCamarasDesdeCarga(int nivelCargado)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool isActive = (i == 0);
            cameras[i].enabled = isActive;

            if (canvasObjects != null && i < canvasObjects.Length)
                canvasObjects[i].SetActive(isActive);
        }

        currentCameraIndex = 0;
        botonCambiarCamara2.gameObject.SetActive(nivelCargado > 2);
        botonCambiarCamara3.gameObject.SetActive(nivelCargado > 1);
        botonCambiarCamara4.gameObject.SetActive(nivelCargado > 3);
        botonCambiarCamara5.gameObject.SetActive(nivelCargado > 3);
        botonCambiarCamara0.interactable = false;
        botonCambiarCamara1.interactable = (nivelCargado > 0);
        botonCambiarCamara2.interactable = (nivelCargado > 2);
        botonCambiarCamara3.interactable = (nivelCargado > 1);
        botonCambiarCamara4.interactable = (nivelCargado > 3);
        botonCambiarCamara5.interactable = (nivelCargado > 3);
    }
    #endregion

    #region Cambio de Cámaras
    public void CambiarCamara(int cameraIndex)
    {
        TaskManager.instance.OcultarListaTareas();
        InventarioManager.Instance.CerrarInventario();

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

        if (cameraIndex == 0 && Tutorial.instance != null && !Tutorial.instance.tutorialSaltado)
        {
            Tutorial.instance.AlVolverACamaraPrincipal();
        }

        ActualizarEstadoBotonesCamaras();
    }

    private void ActualizarEstadoBotonesCamaras()
    {
        if (Gnomos.instance != null && Gnomos.instance.animacionEjecutada == false && Gnomos.instance.desorganizarPendiente)
            return;

        botonCambiarCamara0.interactable = true;
        botonCambiarCamara1.interactable = true;
        botonCambiarCamara2.interactable = true;
        botonCambiarCamara3.interactable = true;
        botonCambiarCamara4.interactable = true;
        botonCambiarCamara5.interactable = true;

        switch (currentCameraIndex)
        {
            case 0: botonCambiarCamara0.interactable = false; break;
            case 1: botonCambiarCamara1.interactable = false; break;
            case 2: botonCambiarCamara2.interactable = false; break;
            case 3: botonCambiarCamara3.interactable = false; break;
            case 4: botonCambiarCamara4.interactable = false; break;
            case 5: botonCambiarCamara5.interactable = false; break;
        }
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
        botonCambiarCamara0.gameObject.SetActive(false);
        botonCambiarCamara1.gameObject.SetActive(false);
        botonCambiarCamara2.gameObject.SetActive(false);
        botonCambiarCamara3.gameObject.SetActive(false);
        botonCambiarCamara4.gameObject.SetActive(false);
        botonCambiarCamara5.gameObject.SetActive(false);
    }

    public void ActivarBotonCamara()
    {
        botonCambiarCamara0.gameObject.SetActive(true);
        botonCambiarCamara1.gameObject.SetActive(true);
        botonCambiarCamara2.gameObject.SetActive(true);
        botonCambiarCamara3.gameObject.SetActive(true);
        botonCambiarCamara4.gameObject.SetActive(true);
        botonCambiarCamara5.gameObject.SetActive(true);
    }

    public void DesactivarBotonCamaraBiblioteca()
    {
        botonCambiarCamara1.gameObject.SetActive(false);
    }

    public void ActivarBotonCamaraBiblioteca()
    {
        botonCambiarCamara1.gameObject.SetActive(true);
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
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        PagesManager.instance.ResetSistema();
        PagesManager.instance.ActivarCategoriaCorrecta();
        EconomyManager.instance.OcultarContenedorDinero();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesactivarPanelReparacion()
    {
        panelReparacion.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }

    public void ActivarPanelPortada()
    {
        panelPortada.SetActive(true);
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        EconomyManager.instance.OcultarContenedorDinero();

        if (bookCoverManager != null)
            StartCoroutine(bookCoverManager.ActualizarTituloLibroDespuesDeFrame());

        bookCoverManager?.ActivarStickersPorSet();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesctivarPanelPortada()
    {
        panelPortada.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }

    public void ActivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        EconomyManager.instance.OcultarContenedorDinero();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesctivarPanelHechizo()
    {
        panelHechizo.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }

    public void ActivarPanelTraduccion()
    {
        panelTraduccion.SetActive(true);
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        EconomyManager.instance.OcultarContenedorDinero();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesctivarPanelTraduccion()
    {
        panelTraduccion.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }

    public void ActivarPanelEncanto()
    {
        panelEncanto.SetActive(true);
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        EconomyManager.instance.OcultarContenedorDinero();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesctivarPanelEncanto()
    {
        panelEncanto.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }


    public void ActivarPanelDonar()
    {
        DonationManager.instance.ActualizarPortada();
        InventarioManager.Instance.OcultarInventarioCompleto();
        HistorialManager.Instance.OcultarBotonAbrirHistorial();
        panelDonar.gameObject.SetActive(true);
        TaskManager.instance.OcultarBotonTareasMinijuegos();
        EconomyManager.instance.OcultarContenedorDinero();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = false;
    }

    public void DesctivarPanelDonar()
    {
        panelDonar.gameObject.SetActive(false);
        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
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