using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonEspacio : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text texto; 
    private Button boton;
    private Image imagen;
    private CriptogramaManager manager;
    private Color colorOriginal;

    private void Awake()
    {
        boton = GetComponent<Button>();
        imagen = GetComponent<Image>();

        if (imagen != null)
            colorOriginal = imagen.color;

        if (boton != null)
            boton.onClick.AddListener(OnClick);

        if (texto == null)
            Debug.LogError("ButtonEspacio: TMP_Text no asignado en inspector");
    }

    public void AsignarManager(CriptogramaManager mgr)
    {
        manager = mgr;
    }

    private void OnClick()
    {
        if (manager != null)
            manager.SeleccionarEspacio(this);
    }

    public void SetLetra(string letra)
    {
        if (texto != null)
            texto.text = letra;
    }

    public void SetSelected(bool selected)
    {
        if (imagen != null)
        {
            imagen.color = selected ? Color.yellow : colorOriginal;
        }
    }

    public void ConfigureAsLetterPlaceholder(CriptogramaManager mgr)
    {
        AsignarManager(mgr);
        if (texto != null) texto.text = "_";
        if (boton != null) boton.interactable = true;
        if (imagen != null) imagen.color = colorOriginal;
    }

    public void ConfigureAsSpace()
    {
        if (texto != null) texto.text = "";
        if (boton != null) boton.interactable = false;

        if (imagen != null)
        {
            Color c = imagen.color;
            c.a = 0f;
            imagen.color = c;
        }
    }

    public void ConfigureAsRevealed(string letra)
    {
        if (texto != null) texto.text = letra;
        if (boton != null) boton.interactable = false;
        if (imagen != null) imagen.color = colorOriginal;
    }
}
