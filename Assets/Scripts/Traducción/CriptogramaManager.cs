using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

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

    public void GenerarCriptogramaPorClave(string claveMensaje)
    {
        if (string.IsNullOrEmpty(claveMensaje)) 
        {
            Debug.LogError("Clave de mensaje vacía en CriptogramaManager.");
            return;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "MensajesCriptograma",
            TableEntryReference = claveMensaje
        };

        localizedString.GetLocalizedStringAsync().Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                string mensaje = handle.Result;
                GenerarCriptograma(mensaje);
            }
            else
            {
                Debug.LogError($"No se pudo cargar el mensaje de criptograma: {claveMensaje}");
            }
        };
    }

    private void GenerarCriptograma(string mensaje)
    {
        if (string.IsNullOrEmpty(mensaje)) return;

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
            espacioGO.SetActive(true); 

            ButtonEspacio be = espacioGO.GetComponent<ButtonEspacio>();
            if (be == null)
            {
                Debug.LogError("Prefab no tiene ButtonEspacio asignado");
                continue;
            }

            be.AsignarManager(this);

            if (c == ' ')
            {
                be.ConfigureAsSpace();
            }
            else
            {
                bool revelar = rnd.NextDouble() < 0.5;

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
            if (string.IsNullOrEmpty(text)) continue;

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
        }
        else
        {
            if (botonEntregar != null) botonEntregar.gameObject.SetActive(false);
        }
    }
}