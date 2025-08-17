using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using System.Collections.Generic;

public class HintsPortada : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonHint;
    [SerializeField] private TMP_Text textoHint;
    [SerializeField] private GameObject panelHint;

    private int indicePistaActual = 0;
    private List<LocalizedString> pistasActuales;

    private void Start()
    {
        if (botonHint != null)
            botonHint.onClick.AddListener(MostrarSiguientePista);

        if (textoHint != null)
            textoHint.gameObject.SetActive(false);

        CargarPistasDePersonaje();
    }

    private void CargarPistasDePersonaje()
    {
        var personaje = CharacterManager.instance?.UltimoPersonajeAtendido;
        if (personaje != null && personaje.tipoDePedido == CharacterAttributes.TipoDePedido.HacerPortada)
        {
            pistasActuales = personaje.pistasPortada;
        }
        else
        {
            pistasActuales = new List<LocalizedString>();
        }
        ReiniciarPistas();
    }

    public void ReiniciarPistas()
    {
        indicePistaActual = 0;
        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
    }

    private void MostrarSiguientePista()
    {
        if (pistasActuales == null || pistasActuales.Count == 0 || textoHint == null) return;

        textoHint.gameObject.SetActive(true);
        panelHint.SetActive(true);

        if (indicePistaActual < pistasActuales.Count)
        {
            LocalizedString pista = pistasActuales[indicePistaActual];
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
        textoHint.text = texto;
    }

    private IEnumerator OcultarHintLuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        textoHint.gameObject.SetActive(false);
        panelHint.SetActive(false);
    }
}
