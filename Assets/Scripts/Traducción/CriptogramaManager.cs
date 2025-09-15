using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CriptogramaManager : MonoBehaviour
{
    public static CriptogramaManager instance;

    [Header("Referencias")]
    public Transform contenedorEspacios;   
    public TMP_Text textoRunas;           
    public GameObject prefabEspacio;       
    public Button botonEntregar;           

    private ButtonEspacio espacioSeleccionado;
    private string mensajeCorrecto;        
    private string mensajeCorrectoSinEspaciosUpper;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerarCriptograma("ESPEJO SUELTA");
        if (botonEntregar != null) botonEntregar.gameObject.SetActive(false);
    }

    public void GenerarCriptograma(string mensaje)
    {
        mensajeCorrecto = mensaje;
        mensajeCorrectoSinEspaciosUpper = RemoveSpacesAndToUpper(mensaje);

        textoRunas.gameObject.SetActive(true);
        textoRunas.text = mensaje;

        foreach (Transform child in contenedorEspacios)
            Destroy(child.gameObject);

        System.Random rnd = new System.Random();

        for (int i = 0; i < mensaje.Length; i++)
        {
            char c = mensaje[i];
            GameObject espacioGO = Instantiate(prefabEspacio, contenedorEspacios);
            ButtonEspacio be = espacioGO.GetComponent<ButtonEspacio>();

            be.AsignarManager(this);

            if (c == ' ')
            {
                be.ConfigureAsSpace();
            }
            else
            {
                bool revelar = rnd.NextDouble() < 0.7;

                if (revelar)
                    be.ConfigureAsRevealed(c.ToString());
                else
                    be.ConfigureAsLetterPlaceholder(this);
            }
        }

        if (botonEntregar != null) botonEntregar.gameObject.SetActive(false);
    }

    private string RemoveSpacesAndToUpper(string s) => s.Replace(" ", "").ToUpperInvariant();

    public void SeleccionarEspacio(ButtonEspacio espacio)
    {
        if (espacioSeleccionado != null)
            espacioSeleccionado.SetSelected(false);

        espacioSeleccionado = espacio;

        if (espacioSeleccionado != null)
            espacioSeleccionado.SetSelected(true);
    }

    public void ColocarLetra(string letra)
    {
        if (espacioSeleccionado == null)
        {
            Debug.Log("CriptogramaManager: no hay espacio seleccionado.");
            return;
        }

        string letterUpper = letra.ToUpperInvariant();
        espacioSeleccionado.SetLetra(letterUpper);
        espacioSeleccionado.SetSelected(false);
        espacioSeleccionado = null;

        VerificarTraduccion();
    }

    private void VerificarTraduccion()
    {
        string resultado = "";
        foreach (Transform child in contenedorEspacios)
        {
            TMP_Text t = child.GetComponentInChildren<TMP_Text>();
            if (t == null) continue;

            string text = t.text.Trim();
            if (string.IsNullOrEmpty(text)) 
                continue;

            if (text == "_" || text == "__") 
            {
                if (botonEntregar != null) botonEntregar.gameObject.SetActive(false);
                return;
            }

            resultado += text;
        }

        string resultadoUpper = resultado.ToUpperInvariant();
        if (resultadoUpper == mensajeCorrectoSinEspaciosUpper)
        {
            if (botonEntregar != null) botonEntregar.gameObject.SetActive(true);
            Debug.Log("Criptograma completado correctamente: botón Entregar activo.");
        }
        else
        {
            if (botonEntregar != null) botonEntregar.gameObject.SetActive(false);
            Debug.Log("Criptograma completado (llenado) pero incorrecto.");
        }
    }
}
