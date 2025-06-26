using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HintsHechizos : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Button botonHint;
    [SerializeField] private TMP_Text textoHint;
    [SerializeField] private GameObject panelHint;

    [Header("Configuración de pistas")]
    [TextArea]
    [SerializeField] private List<string> pistas = new List<string>();

    private int indicePistaActual = 0;

    private void Start()
    {
        if (botonHint != null)
            botonHint.onClick.AddListener(MostrarSiguientePista);

        if (textoHint != null)
            textoHint.gameObject.SetActive(false);

        pistas.Add("Ordená las runas en el orden correcto para activar el hechizo.");
        pistas.Add("¿Probaste empezar con la runa de Tierra?");
        pistas.Add("Sombra es bastante estable... tal vez vaya en el medio.");
        pistas.Add("Protección suena a algo que combinaría con Fuego, ¿no?");
        pistas.Add("Tierra → Sombra → Fuego podría ser una buena combinación...");
    }

    public void ReiniciarPistas()
    {
        indicePistaActual = 0;
        if (textoHint != null)
            textoHint.gameObject.SetActive(false);
    }

    private void MostrarSiguientePista()
    {
        if (pistas.Count == 0 || textoHint == null) return;

        textoHint.gameObject.SetActive(true); 
        panelHint.SetActive(true);

        if (indicePistaActual < pistas.Count)
        {
            textoHint.text = pistas[indicePistaActual];
            indicePistaActual++;
        }
        else
        {
           // textoHint.text = "No hay más pistas disponibles.";
            indicePistaActual = 0; 
        }

        StopAllCoroutines();
        StartCoroutine(OcultarHintLuego(6f));
    }


    private IEnumerator OcultarHintLuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        textoHint.gameObject.SetActive(false);
        panelHint.SetActive(false); 
    }
}