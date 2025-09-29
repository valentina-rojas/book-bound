using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Analytics;
using static EventManager; 

public class Cinematica : MonoBehaviour
{
    [Header("UI")]
    public Image imagenUI;
    public GameObject[] gruposDeTexto;

    [Header("Cinemática")]
    public Sprite[] imagenes;
    public float tiempoPorImagen = 2f;

    [Header("Opciones")]
    public string escenaSiguiente = "Gameplay";

    private int indiceActual = 0;
    private bool cinematicaCerrada = false;
    private float tiempoInicio;

    void Start()
    {
        Time.timeScale = 1f;
        tiempoInicio = Time.time;

        if (imagenes != null && imagenes.Length > 0 && imagenUI != null)
        {
            StartCoroutine(ReproducirCinematica());
        }
        else
        {
            Debug.LogWarning("Faltan referencias en la cinemática (imagenes o imagenUI).");
        }
    }

    IEnumerator ReproducirCinematica()
    {
        while (indiceActual < imagenes.Length)
        {
            imagenUI.sprite = imagenes[indiceActual];

            foreach (var grupo in gruposDeTexto)
            {
                if (grupo != null) grupo.SetActive(false);
            }

            if (indiceActual < gruposDeTexto.Length && gruposDeTexto[indiceActual] != null)
            {
                gruposDeTexto[indiceActual].SetActive(true);
            }

            yield return new WaitForSeconds(tiempoPorImagen);
            indiceActual++;
        }

        CerrarCinematica(); 
    }

    public void OmitirCinematica()
    {
        if (cinematicaCerrada) return;
        cinematicaCerrada = true;

        RegistrarEventoCinematica(true); 
        ChangeScene(escenaSiguiente);
    }

    public void CerrarCinematica()
    {
        if (cinematicaCerrada) return;
        cinematicaCerrada = true;

        RegistrarEventoCinematica(false); 
        ChangeScene(escenaSiguiente);
    }

    private void RegistrarEventoCinematica(bool saltada)
    {
        CinematicaEvent cinEvent = new CinematicaEvent();
        cinEvent.skip = saltada;  

#if !UNITY_EDITOR
    AnalyticsService.Instance.RecordEvent(cinEvent);
#else
        Debug.Log($"[ANALYTICS] CinematicaEvent registrado con skip={saltada}");
#endif
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
