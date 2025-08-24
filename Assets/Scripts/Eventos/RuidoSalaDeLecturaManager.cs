using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RuidoSalaDeLecturaManager : MonoBehaviour
{
    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioRuido;

    [Header("Sonido Silenciar")]
    [SerializeField] private AudioClip sonidoSilenciar;

    private AudioSource audioSourceEfectos;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject botonDetenerRuido;
    [SerializeField] private GameObject[] globosDialogo;
    #endregion

    #region Variables Privadas
    private bool eventoActivo = false;
    private bool esperandoProbabilidad = false;
    private Coroutine corutinaEvento;
    private CatDialogues catDialogues;
    private bool eventosPermitidos = true;
    #endregion

    #region Ciclo de Vida
    private void Awake()
    {
        catDialogues = Object.FindFirstObjectByType<CatDialogues>();
        if (catDialogues == null)
            Debug.LogWarning("No se encontró CatDialogues en la escena.");
    }

    private void Start()
    {
        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        audioSourceEfectos = gameObject.AddComponent<AudioSource>();
    }
    #endregion

    #region Evento Sala Ruidosa
    public void PermitirEventos()
    {
        eventosPermitidos = true;
        IntentarActivarSalaRuidosa();
    }

    public void IntentarActivarSalaRuidosa()
    {
        if (!eventosPermitidos) 
        {
            return;
        }

        if (eventoActivo || esperandoProbabilidad)
            return;

        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager no está disponible.");
            return;
        }

        if (GameManager.instance.nivelActual <= 3 ||
            TaskManager.instance == null)
        {
            return;
        }

        esperandoProbabilidad = true;
        corutinaEvento = StartCoroutine(CheckearProbabilidadEvento());
    }

    private IEnumerator CheckearProbabilidadEvento()
    {
        while (!eventoActivo)
        {
            float tiempoEspera = Random.Range(30f, 60f);
            yield return new WaitForSeconds(tiempoEspera);

            float chance = Random.value;

            if (chance <= 0.25f)
            {
                ActivarEventoSalaRuidosa();
                yield break;
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

        foreach (GameObject obj in globosDialogo)
        {
            if (obj != null)
                obj.SetActive(true);
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

        catDialogues?.IniciarDialogoExtraDesdeLista(
            new string[] { "SalaRuidosa" },
            "Extra"
        );
    }
    #endregion

    #region Control Silenciar
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
    #endregion

    #region Desactivación del Evento
    public void DesactivarSalaRuidosa()
    {
        if (!eventoActivo)
            return;

        if (audioRuido == null)
        {
            Debug.LogWarning("AudioSource no asignado.");
            return;
        }

        foreach (GameObject obj in globosDialogo)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        audioRuido.Stop();
        eventoActivo = false;

        if (botonDetenerRuido != null)
            botonDetenerRuido.SetActive(false);

        catDialogues?.FinalizarDialogo();

        if (eventosPermitidos)
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
        eventosPermitidos = false; 
    }
    #endregion
}