using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections;

public class HistorialManager : MonoBehaviour
{
    private UIManager uiManager;
    private List<string> librosPrestados = new List<string>();
    private bool historialCargado = false; 

    [Header("Mensajes Localizados")]
    public LocalizedString mensajeHistorialVacio;
    public LocalizedString mensajeLibrosVacio;

    private void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
        {
            if (uiManager.GetBotonCerrarHistorial() != null)
                uiManager.GetBotonCerrarHistorial().onClick.AddListener(CerrarTodo);

            if (uiManager.GetBotonAbrirHistorial() != null)
                uiManager.GetBotonAbrirHistorial().onClick.AddListener(AbrirTodo);

            uiManager.GetPanelHistorial().SetActive(false);
        }
    }

    public void AbrirTodo()
    {
        if (uiManager == null) return;

        if (historialCargado)
        {
            uiManager.GetPanelHistorial().SetActive(true);
            return;
        }

        StartCoroutine(CargarPalabrasResaltadas((palabrasParaNegrita) =>
        {
            StartCoroutine(MostrarHistorialConPalabras(palabrasParaNegrita));
        }));
        StartCoroutine(MostrarLibrosPrestados());
        uiManager.GetPanelHistorial().SetActive(true);
        historialCargado = true;
    }

    public void CerrarTodo()
    {
        if (uiManager != null && uiManager.GetPanelHistorial() != null)
        {
            uiManager.GetPanelHistorial().SetActive(false);
            Debug.Log("Panel de historial cerrado");
            historialCargado = false; 
        }
    }

    #region Historial de Pedidos

    private IEnumerator CargarPalabrasResaltadas(System.Action<List<string>> callback)
    {
        var tableLoading = LocalizationSettings.StringDatabase.GetTableAsync("PalabrasResaltadas");
        yield return tableLoading;

        var table = tableLoading.Result as StringTable;
        var palabras = new List<string>();

        if (table == null)
        {
            Debug.LogWarning("No se encontró la tabla 'PalabrasResaltadas'.");
            callback(palabras);
            yield break;
        }

        foreach (var entry in table)
        {
            if (!string.IsNullOrEmpty(entry.Value.LocalizedValue))
            {
                palabras.Add(entry.Value.LocalizedValue);
            }
        }

        callback(palabras);
    }

    private IEnumerator MostrarHistorialConPalabras(List<string> palabrasParaNegrita)
    {
        var personajes = CharacterManager.instance.GetPersonajesAtendidos();
        LimpiarHistorialUI();

        if (personajes == null || personajes.Count == 0)
        {
            bool mensajeMostrado = false;
            mensajeHistorialVacio.StringChanged -= MostrarMensajeHistorialVacioHandler; // prevenir múltiples suscripciones
            mensajeHistorialVacio.StringChanged += MostrarMensajeHistorialVacioHandler;
            yield break;
        }

        HashSet<string> entradasExistentes = new HashSet<string>();

        foreach (var personaje in personajes)
        {
            bool done = false;
            string descripcionTraducida = "";

            yield return StartCoroutine(personaje.GetDescripcionPedidoLocalized((desc) =>
            {
                descripcionTraducida = ResaltarEnNegrita(desc, palabrasParaNegrita);
                done = true;
            }));
            while (!done) yield return null;

            string claveEntrada = personaje.nombreDelCliente + "|" + descripcionTraducida;
            if (entradasExistentes.Contains(claveEntrada))
            {
                Debug.Log("Entrada duplicada detectada. No se agrega.");
                continue;
            }

            entradasExistentes.Add(claveEntrada);

            GameObject entrada = Instantiate(uiManager.GetPrefabEntradaHistorial(), uiManager.GetHistorialContent());
            entrada.transform.SetSiblingIndex(0);
            TMP_Text[] textos = entrada.GetComponentsInChildren<TMP_Text>();

            if (textos.Length >= 2)
            {
                string nombreTraducido = personaje.nombreDelCliente;

                yield return StartCoroutine(personaje.GetNombreClienteLocalized(nombre =>
                {
                    nombreTraducido = nombre;
                }));

                textos[0].text = nombreTraducido;
                textos[1].text = descripcionTraducida;
            }
        }
    }

    private void MostrarMensajeHistorialVacioHandler(string mensaje)
    {
        MostrarMensajeHistorialVacio(mensaje);
    }

    private string ResaltarEnNegrita(string texto, List<string> palabras)
    {
        if (string.IsNullOrEmpty(texto) || palabras == null || palabras.Count == 0)
            return texto;

        foreach (var palabra in palabras)
        {
            if (!string.IsNullOrEmpty(palabra))
            {
                texto = texto.Replace(palabra, $"<b><color=#e82e2e>{palabra}</color></b>");
            }
        }
        return texto;
    }

    public void TachadoUltimaEntrada()
    {
        if (uiManager == null) return;

        var contenido = uiManager.GetHistorialContent();
        if (contenido.childCount == 0) return;

        Transform ultimaEntrada = contenido.GetChild(0);
        TMP_Text[] textos = ultimaEntrada.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            if (!textos[1].text.StartsWith("<s>"))
            {
                textos[1].text = $"<s>{textos[1].text}</s>";
            }
        }
    }

    private void LimpiarHistorialUI()
    {
        if (uiManager.GetHistorialContent() == null) return;

        foreach (Transform child in uiManager.GetHistorialContent())
        {
            Destroy(child.gameObject);
        }
    }

    private void MostrarMensajeHistorialVacio(string mensaje)
    {
        GameObject entradaVacia = Instantiate(uiManager.GetPrefabEntradaHistorial(), uiManager.GetHistorialContent());
        TMP_Text[] textos = entradaVacia.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            textos[0].text = mensaje;
            textos[1].text = "";
        }
    }

    #endregion

    #region Libros Prestados
    private IEnumerator MostrarLibrosPrestados()
    {
        var personajes = CharacterManager.instance.GetPersonajesAtendidos();
        LimpiarLibrosUI();
        librosPrestados.Clear();

        if (personajes == null || personajes.Count == 0)
        {
            mensajeLibrosVacio.StringChanged -= MostrarMensajeLibrosVacioHandler;
            mensajeLibrosVacio.StringChanged += MostrarMensajeLibrosVacioHandler;
            Debug.Log("No hay personajes atendidos");
            yield break; 
        }
        else
        {
            bool hayLibros = false;
            foreach (var personaje in personajes)
            {
                Debug.Log($"Revisando personaje: {personaje.nombreDelCliente} - Libro: '{personaje.tituloLibroPrestado}'");

                if (!string.IsNullOrEmpty(personaje.tituloLibroPrestado))
                {
                    hayLibros = true;

                    GameObject entrada = Instantiate(uiManager.GetPrefabEntradaLibro(), uiManager.GetLibrosContent());
                    entrada.transform.SetSiblingIndex(0);
                    TMP_Text[] textos = entrada.GetComponentsInChildren<TMP_Text>();

                    if (textos.Length >= 2)
                    {
                        string nombreTraducido = personaje.nombreDelCliente;

                        yield return StartCoroutine(personaje.GetNombreClienteLocalized(nombre =>
                        {
                            nombreTraducido = nombre;
                        }));

                        textos[0].text = nombreTraducido;

                        LocalizedString localizedTitle = new LocalizedString("TitulosLibros", personaje.tituloLibroPrestado);
                        localizedTitle.StringChanged -= LocalizedTitle_StringChanged;
                        localizedTitle.StringChanged += LocalizedTitle_StringChanged;

                        void LocalizedTitle_StringChanged(string tituloTraducido)
                        {
                            textos[1].text = tituloTraducido;
                            if (!librosPrestados.Contains(tituloTraducido))
                            {
                                librosPrestados.Add(tituloTraducido);
                                Debug.Log($"Libro prestado mostrado: {tituloTraducido}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Prefab de entrada libro necesita al menos 2 TMP_Text.");
                    }
                }
            }

            if (!hayLibros)
            {
                mensajeLibrosVacio.StringChanged -= MostrarMensajeLibrosVacioHandler;
                mensajeLibrosVacio.StringChanged += MostrarMensajeLibrosVacioHandler;
                Debug.Log("No se encontraron libros prestados en personajes.");
            }
        }

        yield break; 
    }

    private void MostrarMensajeLibrosVacioHandler(string mensaje)
    {
        MostrarMensajeLibrosVacio(mensaje);
    }

    public List<string> GetLibrosPrestados()
    {
        return new List<string>(librosPrestados);
    }

    public void RemoverLibroPrestado(string titulo)
    {
        if (librosPrestados.Contains(titulo))
        {
            librosPrestados.Remove(titulo);
            Debug.Log($"Libro '{titulo}' eliminado de la lista de libros prestados.");
        }
    }

    private void LimpiarLibrosUI()
    {
        if (uiManager.GetLibrosContent() == null) return;

        foreach (Transform child in uiManager.GetLibrosContent())
        {
            Destroy(child.gameObject);
        }
    }

    private void MostrarMensajeLibrosVacio(string mensaje)
    {
        GameObject entradaVacia = Instantiate(uiManager.GetPrefabEntradaLibro(), uiManager.GetLibrosContent());
        TMP_Text[] textos = entradaVacia.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            textos[0].text = mensaje;
            textos[1].text = "";
            Debug.Log("Mostrando mensaje de libros vacío");
        }
    }
    #endregion
}