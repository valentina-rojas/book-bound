using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HistorialManager : MonoBehaviour
{
    private UIManager uiManager;
    private List<string> librosPrestados = new List<string>();

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

        MostrarHistorial();
        MostrarLibrosPrestados();
        uiManager.GetPanelHistorial().SetActive(true);
    }

    public void CerrarTodo()
    {
        if (uiManager != null && uiManager.GetPanelHistorial() != null)
        {
            uiManager.GetPanelHistorial().SetActive(false);
            Debug.Log("Panel de historial cerrado");
        }
    }

    #region Historial de Pedidos

    private string ResaltarEnNegrita(string texto, List<string> palabras)
    {
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

    private void MostrarHistorial()
    {
        var personajes = CharacterManager.instance.GetPersonajesAtendidos();
        LimpiarHistorialUI();

        if (personajes == null || personajes.Count == 0)
        {
            MostrarMensajeHistorialVacio("No hay historial para mostrar.");
        }
        else
        {
            List<string> palabrasParaNegrita = new List<string>
            {
                "La Dama de los Secretos Perdidos", "vengarse", "asustar", "cama", "reparar", "crear", "portada", "libro",
                "hechizar", "prueba", "conocimientos", "equilibrio", "luz", "oscuridad", "El Guardián del Bosque Azul",
                "relatos", "invisible", "no dicho", "Compendio de Plantas Susurrantes"
            };

            foreach (var personaje in personajes)
            {
                var contenido = uiManager.GetHistorialContent();
                if (contenido.childCount > 0)
                {
                    Transform entradaAnterior = contenido.GetChild(0);
                    TMP_Text[] textosAnteriores = entradaAnterior.GetComponentsInChildren<TMP_Text>();
                    if (textosAnteriores.Length >= 2 && !textosAnteriores[1].text.StartsWith("<s>"))
                    {
                        textosAnteriores[1].text = $"<s>{textosAnteriores[1].text}</s>";
                    }
                }

                GameObject entrada = Instantiate(uiManager.GetPrefabEntradaHistorial(), uiManager.GetHistorialContent());
                entrada.transform.SetSiblingIndex(0);
                TMP_Text[] textos = entrada.GetComponentsInChildren<TMP_Text>();

                if (textos.Length >= 2)
                {
                    textos[0].text = personaje.nombreDelCliente;
                    textos[1].text = ResaltarEnNegrita(personaje.descripcionPedido, palabrasParaNegrita);
                }
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

    private void MostrarLibrosPrestados()
    {
        var personajes = CharacterManager.instance.GetPersonajesAtendidos();
        LimpiarLibrosUI();

        librosPrestados.Clear();

        if (personajes == null || personajes.Count == 0)
        {
            MostrarMensajeLibrosVacio("No hay libros prestados para mostrar.");
            Debug.Log("No hay personajes atendidos");
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
                        textos[0].text = personaje.nombreDelCliente;
                        textos[1].text = personaje.tituloLibroPrestado;
                        Debug.Log($"Libro prestado mostrado: {personaje.tituloLibroPrestado}");
                        librosPrestados.Add(personaje.tituloLibroPrestado);
                    }
                    else
                    {
                        Debug.LogWarning("Prefab de entrada libro necesita al menos 2 TMP_Text.");
                    }
                }
            }

            if (!hayLibros)
            {
                MostrarMensajeLibrosVacio("No hay libros prestados para mostrar.");
                Debug.Log("No se encontraron libros prestados en personajes.");
            }
        }

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