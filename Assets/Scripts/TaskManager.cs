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

    #region Variables Paneles y Botones
    [Header("Paneles y Botones")]
    public GameObject panelTareas;
    public Button botonAbrirLista;
    public Button botonCerrarLista;
    public Button botonAbrirTienda;

    private bool tareasYaCompletadas = false;
    private bool tiendaAbiertaEnEsteNivel = false;
    #endregion

    #region Variables Audio
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoTareaCompletada;
    #endregion

    #region Variables Configuración de Niveles
    [Header("Configuración de niveles")]
    public List<NivelDeTareas> nivelesDeTareas;

    private List<TMP_Text> textosTareas;
    private List<bool> tareasCompletadas;
    private bool tiendaAbiertaPorPrimeraVez = false;
    #endregion

    #region Unity Callbacks
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
    #endregion

    #region Inicialización
    public void InicializarTareasParaNivel()
    {
        tareasYaCompletadas = false;
        tiendaAbiertaEnEsteNivel = false;

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

                LocalizedStrikethrough strikethrough = texto.GetComponent<LocalizedStrikethrough>();
                if (strikethrough != null)
                    strikethrough.Reiniciar();
            }
            tareasCompletadas.Add(false);
        }

        panelTareas.SetActive(false);
        botonAbrirLista.gameObject.SetActive(true);
        botonCerrarLista.gameObject.SetActive(false);
        botonAbrirTienda.gameObject.SetActive(false);

        if (TendCat.instance != null)
            TendCat.instance.ActualizarVisibilidadObjetos();
        DragRegadera.instance.ActualizarVisibilidadRegadera();
    }
    #endregion

    #region Métodos de UI
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
    #endregion

    #region Lógica de Tienda
    private void OnClickAbrirTienda()
    {
        OcultarListaTareas();
        botonAbrirTienda.gameObject.SetActive(false);
        tiendaAbiertaEnEsteNivel = true;

        RuidoSalaDeLecturaManager ruido = FindFirstObjectByType<RuidoSalaDeLecturaManager>();
        if (ruido != null)
            ruido.IntentarActivarSalaRuidosa();

        Gnomos gnomos = FindFirstObjectByType<Gnomos>();
        if (gnomos != null)
            gnomos.IntentarActivarEventoGnomos();

        if (!tiendaAbiertaPorPrimeraVez)
        {
            tiendaAbiertaPorPrimeraVez = true;

            if (Tutorial.instance != null && !Tutorial.instance.tutorialSaltado)
            {
                Tutorial.instance.AvanzarAlSiguientePaso();
            }
        }
    }

    public void HabilitarBotonTienda()
    {
        if (botonAbrirTienda != null)
            botonAbrirTienda.gameObject.SetActive(true);
    }

    public bool SeAbrioTiendaAlMenosUnaVez()
    {
        return tiendaAbiertaPorPrimeraVez;
    }

    public void RestaurarEstadoTienda(bool estadoTienda)
    {
        tiendaAbiertaPorPrimeraVez = estadoTienda;

        if (tiendaAbiertaPorPrimeraVez && TodasLasTareasCompletadas())
        {
            HabilitarBotonTienda();
        }
    }
    #endregion

    #region Lógica de Tareas
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

            if (animacionLapiz != null && botonAbrirLista.gameObject.activeInHierarchy)
                animacionLapiz.ReproducirAnimacion();

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

    private IEnumerator TacharYReproducirSonido(int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        TacharTexto(id);
        audioSource.PlayOneShot(sonidoTareaCompletada);
        RevisarTareas();
    }

    private void TacharTexto(int id)
    {
        if (id < 0 || id >= textosTareas.Count) return;

        TMP_Text texto = textosTareas[id];
        if (texto != null)
        {
            LocalizedStrikethrough strikethrough = texto.GetComponent<LocalizedStrikethrough>();
            if (strikethrough != null)
            {
                strikethrough.ActivarTachado();
            }
            else
            {
                texto.text = "<s>" + texto.text + "</s>";
            }
        }
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

        if (GameManager.instance != null)
        {
            if (GameManager.instance.nivelActual == 1)
            {
                if (Tutorial.instance != null && !Tutorial.instance.tutorialSaltado)
                {
                    Tutorial.instance.MostrarFlechaVolver();
                }
                else
                {
                    if (botonAbrirTienda != null && !tiendaAbiertaEnEsteNivel)
                    {
                        panelTareas.SetActive(true);
                        botonAbrirLista.gameObject.SetActive(false);
                        botonAbrirTienda.gameObject.SetActive(true);
                    }
                }
            }
            else 
            {
                if (botonAbrirTienda != null && !tiendaAbiertaEnEsteNivel)
                {
                    panelTareas.SetActive(true);
                    botonAbrirLista.gameObject.SetActive(false);
                    botonAbrirTienda.gameObject.SetActive(true);
                }
            }
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

    public bool EsTareaActiva(int id)
    {
        if (textosTareas == null || id < 0 || id >= textosTareas.Count)
            return false;

        return textosTareas[id].gameObject.activeSelf;
    }
    #endregion
}