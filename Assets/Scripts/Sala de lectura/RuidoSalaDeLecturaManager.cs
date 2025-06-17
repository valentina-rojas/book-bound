using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RuidoSalaDeLecturaManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioRuido;

    [Header("Sonido Silenciar")]
    [SerializeField] private AudioClip sonidoSilenciar;

    [Header("UI")]
    [SerializeField] private GameObject botonDetenerRuido;

    private bool eventoActivo = false;
    private AudioSource audioSourceEfectos;
    private CatDialogues catDialogues;

    private void Awake()
    {
        catDialogues = FindObjectOfType<CatDialogues>();
        if (catDialogues == null)
            Debug.LogWarning("❗ No se encontró CatDialogues en la escena.");
    }

    private void Start()
    {
        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        audioSourceEfectos = gameObject.AddComponent<AudioSource>();
    }

    public void IntentarActivarSalaRuidosa()
    {
        Debug.Log("🟩 IntentarActivarSalaRuidosa fue llamado.");

        if (eventoActivo)
            return;

        if (GameManager.instance == null)
        {
            Debug.LogWarning("❗ GameManager no está disponible.");
            return;
        }

        if (GameManager.instance.nivelActual <= 3)
        {
            Debug.Log("ℹ️ Nivel demasiado bajo para activar sala ruidosa.");
            return;
        }

        ActivarEventoSalaRuidosa();
    }

    private void ActivarEventoSalaRuidosa()
    {
        if (audioRuido == null)
        {
            Debug.LogWarning("❗ AudioSource no asignado.");
            return;
        }

        eventoActivo = true;

        if (!audioRuido.gameObject.activeSelf)
            audioRuido.gameObject.SetActive(true);

        if (!audioRuido.enabled)
            audioRuido.enabled = true;

        audioRuido.loop = true;
        audioRuido.Play();

        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(true);

        Debug.Log("📢 Evento de sala ruidosa activado.");

        if (catDialogues != null)
        {
            catDialogues.IniciarDialogoExtra("Esta es una librería, no una discoteca. ¡Ve a callarlos!");
        }
    }

    public void SilenciarConSonido()
    {
        if (eventoActivo && sonidoSilenciar != null && audioSourceEfectos != null)
        {
            StartCoroutine(ReproducirSonidoYDesactivar());
        }
        else
        {
            DesactivarSalaRuidosa();
        }
    }

    private IEnumerator ReproducirSonidoYDesactivar()
    {
        audioSourceEfectos.PlayOneShot(sonidoSilenciar);
        yield return new WaitForSeconds(sonidoSilenciar.length);
        DesactivarSalaRuidosa();
    }

    public void DesactivarSalaRuidosa()
    {
        if (!eventoActivo)
            return;

        if (audioRuido == null)
        {
            Debug.LogWarning("❗ AudioSource no asignado.");
            return;
        }

        audioRuido.Stop();
        eventoActivo = false;

        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        Debug.Log("📴 Evento de sala ruidosa desactivado.");

        if (catDialogues != null)
        {
            catDialogues.FinalizarDialogo();
        }
    }
}