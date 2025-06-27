using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class NivelDeTareas
{
    public List<TMP_Text> textosTareas; 
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;

    public AnimacionLapiz animacionLapiz;

    [Header("Paneles y Botones")]
    public GameObject panelTareas;
    public Button botonAbrirLista;
    public Button botonCerrarLista;
    public Button botonAbrirTienda;
    private bool tareasYaCompletadas = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoTareaCompletada;

    [Header("Configuración de niveles")]
    public List<NivelDeTareas> nivelesDeTareas;

    private List<TMP_Text> textosTareas;
    private List<bool> tareasCompletadas;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        panelTareas.SetActive(false);
        botonAbrirLista.gameObject.SetActive(false);
        botonCerrarLista.gameObject.SetActive(false);

        botonAbrirLista.onClick.AddListener(MostrarTareas);
        botonCerrarLista.onClick.AddListener(OcultarListaTareas);
        botonAbrirTienda.onClick.AddListener(OnClickAbrirTienda);

        InicializarTareasParaNivel();
    }

    public void InicializarTareasParaNivel()
    {
        tareasYaCompletadas = false; 

        int nivelIndex = GameManager.instance.nivelActual - 1;

        if (nivelIndex < 0 || nivelIndex >= nivelesDeTareas.Count)
        {
            Debug.LogWarning("Nivel fuera de rango en la lista de tareas: " + nivelIndex);
            return;
        }

        foreach (NivelDeTareas nivel in nivelesDeTareas)
        {
            foreach (TMP_Text texto in nivel.textosTareas)
            {
                if (texto != null)
                    texto.gameObject.SetActive(false);
            }
        }

        textosTareas = nivelesDeTareas[nivelIndex].textosTareas;
        tareasCompletadas = new List<bool>();

        for (int i = 0; i < textosTareas.Count; i++)
        {
            TMP_Text texto = textosTareas[i];
            if (texto != null)
            {
                texto.gameObject.SetActive(true);
                string textoPlano = texto.text.Replace("<s>", "").Replace("</s>", "");
                texto.text = textoPlano;
            }
            tareasCompletadas.Add(false);
        }

        panelTareas.SetActive(false);
        botonAbrirLista.gameObject.SetActive(true);
        botonCerrarLista.gameObject.SetActive(false);
        botonAbrirTienda.gameObject.SetActive(false);

        if (TendCat.instance != null)
            TendCat.instance.ActualizarVisibilidadObjetos();
    }

    public void MostrarTareas()
    {
        panelTareas.SetActive(true);
        botonAbrirLista.gameObject.SetActive(false);
        botonCerrarLista.gameObject.SetActive(true);
    }

    public void OcultarListaTareas()
    {
        panelTareas.SetActive(false);
        botonAbrirLista.gameObject.SetActive(true);
        botonCerrarLista.gameObject.SetActive(false);
    }

    public void OcultarBotonTareas()
    {
        panelTareas.SetActive(false);
        botonAbrirLista.gameObject.SetActive(false);
    }

    private void OnClickAbrirTienda()
    {
        Debug.Log("Tienda abierta");
        botonAbrirTienda.gameObject.SetActive(false);
    }

    public void CompletarTareaPorID(int id)
    {
        if (id < 0 || id >= tareasCompletadas.Count)
        {
            Debug.LogWarning("ID de tarea inválido: " + id);
            return;
        }

        if (!tareasCompletadas[id])
        {
            tareasCompletadas[id] = true;

           // if (animacionLapiz != null)
          //  animacionLapiz.ReproducirAnimacion();

             if (animacionLapiz != null && botonAbrirLista.gameObject.activeInHierarchy)
            {
            animacionLapiz.ReproducirAnimacion();
            }

            if (audioSource != null && sonidoTareaCompletada != null)
            {
                StartCoroutine(TacharYReproducirSonido(id, 0.5f));
            }
            else
            {
                TacharTexto(id);
            }
        }

        RevisarTareas();
    }
    private void RevisarTareas()
    {
        if (tareasYaCompletadas) return; 

        foreach (bool completada in tareasCompletadas)
        {
            if (!completada)
                return;
        }

        tareasYaCompletadas = true; 

        Debug.Log("¡Todas las tareas de este nivel están completas!");

        if (botonAbrirTienda != null)
        {
            panelTareas.SetActive(true);
            botonAbrirLista.gameObject.SetActive(false);
            botonAbrirTienda.gameObject.SetActive(true);
        }
    }

    public bool TodasLasTareasCompletadas()
    {
        foreach (bool tarea in tareasCompletadas)
        {
            if (!tarea)
                return false;
        }
        return true;
    }

    public void ReiniciarTareas()
    {
        InicializarTareasParaNivel();
    }

    private IEnumerator TacharYReproducirSonido(int id, float delay)
    {
        yield return new WaitForSeconds(delay);

        TacharTexto(id);
        audioSource.PlayOneShot(sonidoTareaCompletada);

        RevisarTareas();
    }

    private void TacharTexto(int id)
    {
        string nombreOriginal = textosTareas[id].text;
        textosTareas[id].text = "<s>" + nombreOriginal + "</s>";
    }

    private IEnumerator ReproducirSonidoTareaConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(sonidoTareaCompletada);
    }
    
    public bool EsTareaActiva(int id)
    {
        if (textosTareas == null || id < 0 || id >= textosTareas.Count)
            return false;

        return textosTareas[id].gameObject.activeSelf;
    }

}
