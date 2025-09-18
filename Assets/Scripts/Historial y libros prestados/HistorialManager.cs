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
    public static HistorialManager Instance;
    private UIManager uiManager;
    private List<LibroPrestado> librosPrestados = new List<LibroPrestado>();
    private HashSet<string> librosDevueltos = new HashSet<string>();
    private bool historialCargado = false;

    [Header("Mensajes Localizados")]
    public LocalizedString mensajeHistorialVacio;
    public LocalizedString mensajeLibrosVacio;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        uiManager = FindFirstObjectByType<UIManager>(); 

        CargarPartida();
    }

    private void Start()
    {
        if (uiManager != null)
        {
            if (uiManager.GetBotonCerrarHistorial() != null)
                uiManager.GetBotonCerrarHistorial().onClick.AddListener(CerrarTodo);

            if (uiManager.GetBotonAbrirHistorial() != null)
                uiManager.GetBotonAbrirHistorial().onClick.AddListener(AbrirTodo);

            uiManager.GetPanelHistorial().SetActive(false);
        }
    }

    public void OcultarBotonAbrirHistorial()
    {
        if (uiManager != null && uiManager.GetBotonAbrirHistorial() != null)
        {
            uiManager.GetBotonAbrirHistorial().gameObject.SetActive(false);
        }
    }

    public void MostrarBotonAbrirHistorial()
    {
        if (uiManager != null && uiManager.GetBotonAbrirHistorial() != null)
        {
            uiManager.GetBotonAbrirHistorial().gameObject.SetActive(true);
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
            historialCargado = false;
        }

        Tutorial tutorial = FindFirstObjectByType<Tutorial>();
        if (tutorial != null && !tutorial.tutorialSaltado) 
        {
            tutorial.AlCerrarHistorial();
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
            mensajeHistorialVacio.StringChanged -= MostrarMensajeHistorialVacioHandler;
            mensajeHistorialVacio.StringChanged += MostrarMensajeHistorialVacioHandler;
            yield break;
        }

        HashSet<string> entradasExistentes = new HashSet<string>();

        for (int i = 0; i < personajes.Count; i++)
        {
            var personaje = personajes[i];
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

                if (i < personajes.Count - 1)
                {
                    textos[1].text = $"<s>{textos[1].text}</s>";
                }
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
                if (!texto.Contains($"<{palabra}>") && !texto.Contains($"</{palabra}>"))
                {
                    texto = texto.Replace(palabra, $"<b><color=#e82e2e>{palabra}</color></b>");
                }
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

        if (textos.Length >= 2 && !textos[1].text.StartsWith("<s>"))
        {
            textos[1].text = $"<s>{textos[1].text}</s>";
        }

        if (contenido.childCount > 1)
        {
            Transform entradaAnterior = contenido.GetChild(1);
            TMP_Text[] textosAnteriores = entradaAnterior.GetComponentsInChildren<TMP_Text>();
            if (textosAnteriores.Length >= 2 && !textosAnteriores[1].text.StartsWith("<s>"))
            {
                textosAnteriores[1].text = $"<s>{textosAnteriores[1].text}</s>";
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
    public IEnumerator MostrarLibrosPrestados()
    {
        LimpiarLibrosUI();

        if (librosPrestados == null || librosPrestados.Count == 0)
        {
            mensajeLibrosVacio.StringChanged -= MostrarMensajeLibrosVacioHandler;
            mensajeLibrosVacio.StringChanged += MostrarMensajeLibrosVacioHandler;
            yield break;
        }

        foreach (var libro in librosPrestados)
        {
            GameObject entrada = Instantiate(uiManager.GetPrefabEntradaLibro(), uiManager.GetLibrosContent());
            entrada.transform.SetSiblingIndex(0);
            TMP_Text[] textos = entrada.GetComponentsInChildren<TMP_Text>();

            if (textos.Length >= 2)
            {
                textos[0].text = libro.cliente;
                LocalizedString localizedTitle = new LocalizedString("TitulosLibros", libro.titulo);
                localizedTitle.StringChanged += (tituloTraducido) =>
                {
                    if (librosDevueltos.Contains(libro.titulo))
                        textos[1].text = $"<s>{tituloTraducido}</s>";
                    else
                        textos[1].text = tituloTraducido;
                };
            }
        }

        yield break;
    }

    public void ActualizarLibrosPrestados()
    {
        var personajes = CharacterManager.instance.GetPersonajesAtendidos();

        if (personajes == null || personajes.Count == 0)
            return;

        foreach (var personaje in personajes)
        {
            if (!string.IsNullOrEmpty(personaje.tituloLibroPrestado))
            {
                if (!librosPrestados.Exists(l => l.titulo == personaje.tituloLibroPrestado))
                {
                    librosPrestados.Add(new LibroPrestado(personaje.nombreDelCliente, personaje.tituloLibroPrestado));
                }
            }
        }
    }

    private void MostrarMensajeLibrosVacioHandler(string mensaje)
    {
        MostrarMensajeLibrosVacio(mensaje);
    }

public List<LibroPrestado> GetLibrosPrestados()
{
    return new List<LibroPrestado>(librosPrestados);
}

public void RemoverLibroPrestado(string titulo)
{
    var libro = librosPrestados.Find(l => l.titulo == titulo);
    if (libro != null)
    {
        librosPrestados.Remove(libro);
        Debug.Log($"Libro '{titulo}' eliminado de la lista de libros prestados.");
    }
}


    public void RegistrarDevolucion(string tituloLibro)
    {
        if (!librosDevueltos.Contains(tituloLibro))
        {
            librosDevueltos.Add(tituloLibro);
            Debug.Log($"Libro '{tituloLibro}' marcado como devuelto.");
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
        }
    }
    #endregion
    public void GuardarPartida(int nivelActual)
    {
        SaveManager.GuardarNivel(nivelActual, librosPrestados);
    }

    public void CargarPartida()
    {
        SaveData data = SaveManager.CargarNivel();
        librosPrestados = new List<LibroPrestado>(data.librosPrestados);
        StartCoroutine(MostrarLibrosPrestados());
    }

}