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
    private bool esperandoProbabilidad = false;
    private Coroutine corutinaEvento;
    private AudioSource audioSourceEfectos;
    private CatDialogues catDialogues;

    private void Awake()
    {
        catDialogues = FindObjectOfType<CatDialogues>();
        if (catDialogues == null)
            Debug.LogWarning("No se encontró CatDialogues en la escena.");
    }

    private void Start()
    {
        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        audioSourceEfectos = gameObject.AddComponent<AudioSource>();
    }

    public void IntentarActivarSalaRuidosa()
    {
        Debug.Log("IntentarActivarSalaRuidosa fue llamado.");

        if (eventoActivo || esperandoProbabilidad)
            return;

        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager no está disponible.");
            return;
        }

        if (GameManager.instance.nivelActual <= 3)
        {
            Debug.Log("Nivel demasiado bajo para activar sala ruidosa.");
            return;
        }

        esperandoProbabilidad = true;
        corutinaEvento = StartCoroutine(CheckearProbabilidadEvento());
    }

    private IEnumerator CheckearProbabilidadEvento()
    {
        Debug.Log("Comienza a esperar posible activación del evento de ruido.");

        while (!eventoActivo)
        {
            float tiempoEspera = Random.Range(30f, 90f);
            Debug.Log($"Esperando {tiempoEspera:F1} segundos antes del próximo intento.");
            yield return new WaitForSeconds(tiempoEspera);

            float chance = Random.value;
            Debug.Log($"Probabilidad obtenida: {chance}");

            if (chance <= 0.2f)
            {
                ActivarEventoSalaRuidosa();
                yield break;
            }
            else
            {
                Debug.Log("No se activó el evento esta vez. Se intentará de nuevo.");
            }
        }
    }

    private void ActivarEventoSalaRuidosa()
    {
        if (audioRuido == null)
        {
            Debug.LogWarning("AudioSource no asignado.");
            return;
        }

        eventoActivo = true;
        esperandoProbabilidad = false;

        if (!audioRuido.gameObject.activeSelf)
            audioRuido.gameObject.SetActive(true);

        if (!audioRuido.enabled)
            audioRuido.enabled = true;

        audioRuido.loop = true;
        audioRuido.Play();

        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(true);

        Debug.Log("Evento de sala ruidosa activado.");

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
            Debug.LogWarning("AudioSource no asignado.");
            return;
        }

        audioRuido.Stop();
        eventoActivo = false;

        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        Debug.Log("Evento de sala ruidosa desactivado.");

        if (catDialogues != null)
        {
            catDialogues.FinalizarDialogo();
        }

        IntentarActivarSalaRuidosa();
    }

    public void CancelarPosibilidadDeEvento()
    {
        if (corutinaEvento != null)
        {
            StopCoroutine(corutinaEvento);
            corutinaEvento = null;
        }

        esperandoProbabilidad = false;
        Debug.Log("Se canceló la posibilidad de que ocurra el evento de sala ruidosa.");
    }
}