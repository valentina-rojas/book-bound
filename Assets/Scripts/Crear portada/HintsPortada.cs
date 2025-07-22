using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class HintsPortada : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonHint;
    [SerializeField] private TMP_Text textoHint;
    [SerializeField] private GameObject panelHint;

    [Header("Configuración")]
    [SerializeField] private string tabla = "HintsPortada";
    [SerializeField] private List<string> clavesPistas = new List<string>
    {
        "Portada_hint_1",
        "Portada_hint_2",
        "Portada_hint_3",
        "Portada_hint_4",
        "Portada_hint_5"
    };

    private int indicePistaActual = 0;

    private void Start()
    {
        if (botonHint != null)
            botonHint.onClick.AddListener(MostrarSiguientePista);

        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
    }

    public void ReiniciarPistas()
    {
        indicePistaActual = 0;
        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
    }

    private void MostrarSiguientePista()
    {
        if (clavesPistas.Count == 0 || textoHint == null) return;

        textoHint.gameObject.SetActive(true);
        panelHint.SetActive(true);

        if (indicePistaActual < clavesPistas.Count)
        {
            string key = clavesPistas[indicePistaActual];
            LocalizedString pista = new LocalizedString { TableReference = tabla, TableEntryReference = key };
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