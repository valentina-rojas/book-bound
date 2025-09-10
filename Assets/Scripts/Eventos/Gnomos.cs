using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gnomos : MonoBehaviour
{
    public static Gnomos instance;
    private bool eventoActivadoHoy = false;

    [Header("Animación Gnomos")]
    public GameObject[] prefabsGnomos;
    public Transform[] spawnPointsCam0;
    public Transform[] targetPointsCam0;
    public float velocidad = 2f;

    [Header("Audio Gnomos")]
    public AudioClip[] risasClips;
    public AudioClip[] extrasClips;
    private List<AudioSource> risasSources = new List<AudioSource>();
    private List<Coroutine> risasCoroutines = new List<Coroutine>();

    public bool animacionEjecutada = false;
    public bool desorganizarPendiente = false;
    private CatDialogues catDialogues;

    private void Awake()
    {
        instance = this;

        catDialogues = Object.FindFirstObjectByType<CatDialogues>();
        if (catDialogues == null)
            Debug.LogWarning("No se encontró CatDialogues en la escena.");

        foreach (var clip in risasClips)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = false;
            risasSources.Add(src);
        }
    }

    public void IntentarActivarEventoGnomos()
    {
        if (GameManager.instance.nivelActual >= 5 && !eventoActivadoHoy)
        {
            float probabilidad = 0.99f;
            if (Random.value <= probabilidad)
            {
                Debug.Log("Evento de gnomos activado.");
                desorganizarPendiente = true;
            }
            else
            {
                Debug.Log("Los gnomos no hicieron travesuras.");
            }

            eventoActivadoHoy = true;
        }
    }

    public void OnCameraChanged(int cameraIndex)
    {
        if (cameraIndex == 0 && desorganizarPendiente && !animacionEjecutada)
        {
            StartCoroutine(MostrarGnomoAnimacion());
        }
        else if (cameraIndex == 1 && animacionEjecutada && desorganizarPendiente)
        {
            EjecutarDesorganizacion();
        }
    }

    private IEnumerator MostrarGnomoAnimacion()
    {
        CameraManager.instance?.DesactivarBotonCamara();

        if (AudioManager.instance != null && AudioManager.instance.sonidoCampanilla != null)
            AudioManager.instance.sonidoCampanilla.Play();

        foreach (var src in risasSources)
        {
            Coroutine c = StartCoroutine(RisaAleatoria(src));
            risasCoroutines.Add(c);
        }

        List<GameObject> gnomosInstanciados = new List<GameObject>();

        for (int i = 0; i < prefabsGnomos.Length; i++)
        {
            GameObject gnomo = Instantiate(prefabsGnomos[i], spawnPointsCam0[i].position, Quaternion.identity);
            Vector3 dir = (targetPointsCam0[i].position - spawnPointsCam0[i].position).normalized;
            gnomo.transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

            gnomosInstanciados.Add(gnomo);
        }

        bool todosLlegaron = false;
        while (!todosLlegaron)
        {
            todosLlegaron = true;
            for (int i = 0; i < gnomosInstanciados.Count; i++)
            {
                GameObject gnomo = gnomosInstanciados[i];
                Vector3 target = targetPointsCam0[i].position;
                Vector3 dir = (target - gnomo.transform.position).normalized;
                gnomo.transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(gnomo.transform.localScale.x), 1, 1);

                gnomo.transform.position = Vector3.MoveTowards(gnomo.transform.position, target, velocidad * Time.deltaTime);
                if (Vector3.Distance(gnomo.transform.position, target) > 0.1f)
                    todosLlegaron = false;
            }
            yield return null;
        }

        yield return StartCoroutine(ReproducirExtrasDurante(2f));

        bool todosRegresaron = false;
        while (!todosRegresaron)
        {
            todosRegresaron = true;
            for (int i = 0; i < gnomosInstanciados.Count; i++)
            {
                GameObject gnomo = gnomosInstanciados[i];
                Vector3 target = spawnPointsCam0[i].position;
                Vector3 dir = (target - gnomo.transform.position).normalized;
                gnomo.transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(gnomo.transform.localScale.x), 1, 1);

                gnomo.transform.position = Vector3.MoveTowards(gnomo.transform.position, target, velocidad * Time.deltaTime);
                if (Vector3.Distance(gnomo.transform.position, target) > 0.1f)
                    todosRegresaron = false;
            }
            yield return null;
        }

        if (AudioManager.instance != null && AudioManager.instance.sonidoCampanilla != null)
            AudioManager.instance.sonidoCampanilla.Play();

        foreach (var gnomo in gnomosInstanciados)
            Destroy(gnomo);
        foreach (var c in risasCoroutines)
            StopCoroutine(c);
        foreach (var src in risasSources)
            src.Stop();
        risasCoroutines.Clear();

        animacionEjecutada = true;

        if (catDialogues != null)
        {
            catDialogues.IniciarDialogoExtraDesdeLista(
                new string[] { "Gnomos1", "Gnomos2" },
                "Extra"
            );
        }

        CameraManager.instance?.ActivarBotonCamara();
    }

    private IEnumerator RisaAleatoria(AudioSource src)
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 1.5f));

        while (true)
        {
            src.pitch = Random.Range(0.9f, 1.1f);
            src.volume = Random.Range(0.6f, 1f);
            src.Play();
            yield return new WaitForSeconds(src.clip.length + Random.Range(0.5f, 2f));
        }
    }

    private IEnumerator ReproducirExtrasDurante(float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            int cantidad = Random.Range(2, 4);

            for (int i = 0; i < cantidad; i++)
            {
                if (extrasClips.Length > 0)
                {
                    AudioClip clip = extrasClips[Random.Range(0, extrasClips.Length)];
                    float volumen = Random.Range(0.6f, 1f);
                    float pitch = Random.Range(0.9f, 1.1f);
                    GameObject tempGO = new GameObject("ExtraSound");
                    tempGO.transform.position = transform.position;
                    AudioSource aSource = tempGO.AddComponent<AudioSource>();
                    aSource.clip = clip;
                    aSource.volume = volumen;
                    aSource.pitch = pitch;
                    aSource.Play();

                    Destroy(tempGO, clip.length);
                }
            }

            float delay = Random.Range(0.5f, 1f);
            tiempo += delay;
            yield return new WaitForSeconds(delay);
        }
    }

    private void EjecutarDesorganizacion()
    {
        if (ShelfManager.instance != null)
        {
            ShelfManager.instance.DesorganizarLibros();
            desorganizarPendiente = false;
            Debug.Log("Libros desorganizados en cámara 1.");
        }
    }

    public void ReiniciarEvento()
    {
        eventoActivadoHoy = false;
        desorganizarPendiente = false;
        animacionEjecutada = false;
    }
}
