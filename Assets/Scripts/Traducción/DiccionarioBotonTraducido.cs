using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiccionarioBotonTraducido : MonoBehaviour
{
    [Header("Letra asociada")]
    public string letra; 

    [Header("Referencias")]
    public TMP_Text textoLetra; 
    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        letra = (letra ?? "").ToUpperInvariant();

        if (textoLetra != null)
            textoLetra.text = ""; 
    }

    public void RevelarLetra()
    {
        if (textoLetra != null)
            textoLetra.text = letra;

        if (boton != null)
            boton.interactable = true;

        gameObject.SetActive(true);
    }

    public void Ocultar()
    {
        if (textoLetra != null)
            textoLetra.text = "";

        if (boton != null)
            boton.interactable = false;

        gameObject.SetActive(false);
    }
}
