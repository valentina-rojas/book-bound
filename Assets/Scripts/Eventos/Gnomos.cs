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

    public bool animacionEjecutada = false; 
    public bool desorganizarPendiente = false; 

    private void Awake()
    {
        instance = this;
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

        yield return new WaitForSeconds(2f);

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

        foreach (var gnomo in gnomosInstanciados)
            Destroy(gnomo);

        animacionEjecutada = true;
        Debug.Log("Animación completa: gnomos entraron y salieron.");
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