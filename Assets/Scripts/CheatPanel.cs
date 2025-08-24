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
        if (Input.GetKeyDown(KeyCode.C))
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

        CatDialogues cat = Object.FindFirstObjectByType<CatDialogues>();
        if (cat != null)
            cat.CancelarDialogo();
        CameraManager.instance?.ActivarCamaraPrincipal();
        CobwebManager.instance?.ReiniciarTelarañas();
        TendCat.instance?.ReiniciarEstado();
        PlantManager.instance?.ReiniciarEstado();
        TaskManager.instance?.ReiniciarTareas();

        gm.StopAllCoroutines();
        gm.StartCoroutine("MostrarCartelInicioDia");

        ActualizarPanel();

        Tutorial tutorial = Object.FindFirstObjectByType<Tutorial>();
        if (tutorial != null)
        {
            tutorial.SaltarTutorial();
        }
    }

    public void ActualizarPanel()
    {
        GameManager gm = GameManager.instance;
        textoNivelActual.text = $"Nivel actual: {gm.nivelActual}";

        botonAnterior.interactable = gm.nivelActual > 1;
        botonSiguiente.interactable = gm.nivelActual < gm.niveles.Length;
    }
}