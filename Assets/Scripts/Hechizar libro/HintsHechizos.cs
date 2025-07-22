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

    [Header("Configuración")]
    [SerializeField] private string tabla = "HintsHechizos";  
    [SerializeField] private List<string> clavesPistas = new List<string>
    {
        "Hechizos_hint_1",
        "Hechizos_hint_2",
        "Hechizos_hint_3",
        "Hechizos_hint_4",
        "Hechizos_hint_5"
    };

    private int indicePistaActual = 0;
    private LocalizedString pista;

    private void Start()
    {
        if (botonHint != null)
            botonHint.onClick.AddListener(MostrarSiguientePista);

        if (textoHint != null)
            textoHint.gameObject.SetActive(false);

        pista = new LocalizedString { TableReference = tabla };
        pista.StringChanged += ActualizarTextoHint;
    }

    private void MostrarSiguientePista()
    {
        if (clavesPistas.Count == 0 || textoHint == null)
            return;

        if (indicePistaActual >= clavesPistas.Count)
            indicePistaActual = 0;

        pista.TableEntryReference = clavesPistas[indicePistaActual];
        pista.RefreshString();

        indicePistaActual++;

        textoHint.gameObject.SetActive(true);
        panelHint.SetActive(true);

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

    private void OnDestroy()
    {
        if (pista != null)
            pista.StringChanged -= ActualizarTextoHint;
    }
}