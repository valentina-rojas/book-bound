using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatPanel : MonoBehaviour
{
    public GameObject panelCheats;
    public TMP_Text textoNivelActual;
    public Button botonAnterior;
    public Button botonSiguiente;

    private void Start()
    {
        panelCheats.SetActive(false);
        ActualizarPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            panelCheats.SetActive(!panelCheats.activeSelf);
            ActualizarPanel();
        }
    }

    public void SiguienteNivel()
    {
        GameManager gm = GameManager.instance;
        if (gm.nivelActual < gm.niveles.Length)
        {
            gm.nivelActual++;
            SaltarANivelDesdeCheat();
        }
    }

    public void NivelAnterior()
    {
        GameManager gm = GameManager.instance;
        if (gm.nivelActual > 1)
        {
            gm.nivelActual--;
            SaltarANivelDesdeCheat();
        }
    }
    private void SaltarANivelDesdeCheat()
    {
        GameManager gm = GameManager.instance;

        // FINALIZAR DIÁLOGO ACTUAL si hay alguno corriendo
        CatDialogues cat = FindFirstObjectByType<CatDialogues>();
        if (cat != null)
            cat.FinalizarDialogo(); // ← esto limpia y cierra el diálogo activo

        // Reiniciar estados
        ShelfManager.instance?.ReiniciarEstado();
        CameraManager.instance?.ActivarCamaraPrincipal();
        CobwebManager.instance?.ReiniciarTelarañas();
        TendCat.instance?.ReiniciarEstado();
        PlantManager.instance?.ReiniciarEstado();
        ShelfManager.instance?.AvanzarContadorDesorden();

        TaskManager.instance?.ReiniciarTareas();

        gm.StopAllCoroutines();
        gm.StartCoroutine("MostrarCartelInicioDia");

        ActualizarPanel();
    }

    public void ActualizarPanel()
    {
        GameManager gm = GameManager.instance;
        textoNivelActual.text = $"Nivel actual: {gm.nivelActual}";

        botonAnterior.interactable = gm.nivelActual > 1;
        botonSiguiente.interactable = gm.nivelActual < gm.niveles.Length;
    }
}
