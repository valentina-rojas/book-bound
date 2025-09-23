using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

public class HintsHechizos : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonHint;
    [SerializeField] private TMP_Text textoHint;
    [SerializeField] private GameObject panelHint;
    [SerializeField] private GameObject contenedorPadre; 

    private int indicePistaActual = 0;
    private List<LocalizedString> pistasActuales;

    private void Start()
    {
        if (botonHint != null)
            botonHint.onClick.AddListener(MostrarSiguientePista);

        InicializarSistema();
        CargarPistasDePersonaje();
        StartCoroutine(VerificarEstadoPanelPeriodicamente());
    }

    private void OnEnable()
    {
        ReactivarSistemaHints();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void InicializarSistema()
    {
        if (contenedorPadre != null)
            contenedorPadre.SetActive(true);

        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
            
        if (panelHint != null)
            panelHint.SetActive(false);
    }

    public void ReactivarSistemaHints()
    {
        Debug.Log("Reactivando sistema de hints de hechizos");
        
        if (contenedorPadre != null)
        {
            contenedorPadre.SetActive(true);
            Debug.Log("Contenedor padre activado: " + contenedorPadre.activeInHierarchy);
        }

        ReiniciarPistas();
        CargarPistasDePersonaje();
    }

    private IEnumerator VerificarEstadoPanelPeriodicamente()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (contenedorPadre != null && !contenedorPadre.activeInHierarchy)
            {
                GameObject panelHechizos = GameObject.FindGameObjectWithTag("PanelHechizos"); 
                if (panelHechizos != null && panelHechizos.activeInHierarchy)
                {
                    Debug.Log("Panel de hechizos activo pero hints desactivados - Reactivando");
                    ReactivarSistemaHints();
                }
            }
        }
    }

    private void CargarPistasDePersonaje()
    {
        var personaje = CharacterManager.instance?.UltimoPersonajeAtendido;
        if (personaje != null && personaje.tipoDePedido == CharacterAttributes.TipoDePedido.HechizarLibro)
        {
            pistasActuales = personaje.pistasHechizo;
            Debug.Log($"Cargadas {pistasActuales?.Count} pistas de hechizos para {personaje.nombreDelCliente}");
        }
        else
        {
            pistasActuales = new List<LocalizedString>();
            Debug.Log("No hay personaje con pedido de hechizos o personaje es null");
        }
        ReiniciarPistas();
    }

    public void ReiniciarPistas()
    {
        indicePistaActual = 0;
        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
            
        if (panelHint != null)
            panelHint.SetActive(false);

        Debug.Log("Pistas de hechizos reiniciadas");
    }

    private void MostrarSiguientePista()
    {
        if (contenedorPadre != null && !contenedorPadre.activeInHierarchy)
        {
            ReactivarSistemaHints();
        }

        if (pistasActuales == null || pistasActuales.Count == 0 || textoHint == null) return;

        textoHint.gameObject.SetActive(true);
        panelHint.SetActive(true);

        if (indicePistaActual < pistasActuales.Count)
        {
            LocalizedString pista = pistasActuales[indicePistaActual];
            pista.StringChanged -= ActualizarTextoHint; 
            pista.StringChanged += ActualizarTextoHint;
            pista.RefreshString();
            indicePistaActual++;
        }
        else
        {
            indicePistaActual = 0;
        }

        StopAllCoroutines();
        StartCoroutine(OcultarHintLuego(6f));
    }

    private void ActualizarTextoHint(string texto)
    {
        if (textoHint != null)
            textoHint.text = texto;
    }

    private IEnumerator OcultarHintLuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
        if (panelHint != null)
            panelHint.SetActive(false);
    }

    public void OnHechizosPanelActivated()
    {
        Debug.Log("Panel de hechizos activado - Reactivando hints");
        ReactivarSistemaHints();
    }
}