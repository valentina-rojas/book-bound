using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HintsPortada : MonoBehaviour
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

        pistas.Add("Pensá en los elementos que la autora mencionó... ¿qué podría ir primero?");
        pistas.Add("Un mapa puede ser una buena base...");
        pistas.Add("Una brújula puede ayudar a orientarse... tal vez cerca del mapa.");
        pistas.Add("¿Te animás a esconder algo especial entre las montañas?");
        pistas.Add("Tal vez un dragón asomándose...");
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
            textoHint.gameObject.SetActive(true);
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