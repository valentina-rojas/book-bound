using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

public class CharacterAttributes : MonoBehaviour
{
    #region Enums
    public enum TipoDePedido
    {
        BuscarLibro,
        RepararLibro,
        HacerPortada,
        HechizarLibro,
        JuegoTrivia,
        Traduccion,
        DonarLibro,
        DevolverLibro,
        EncantarLibro
    }

    public enum Hechizo
    {
        Ninguno,
        Sellado,
        Proteccion,
        Traduccion,
        Restauracion,
        Comunicacion
    }

    public enum TipoEncanto
    {
        Ninguno,
        MalDeOjo,
        Resfriado,
        Hongos,
        Verrugas
    }
    #endregion

    #region Propiedades Generales
    public TipoDePedido tipoDePedido;

    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesInicio;
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesBuena;
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesMala;

    public Sprite spriteRespuestaBuena;
    public Sprite spriteRespuestaMala;

    public int libroDeseadoID;
    public string tipoPreferido;

    public string nombreDelCliente;

    [Tooltip("Clave de la tabla 'DescripcionPedidos'")]
    public string descripcionPedido; 
    #endregion

    #region Localización
    public IEnumerator GetDialogueInicioLocalized(System.Action<List<string>> callback)
    {
        yield return GetLocalizedLinesFromTable("DialogosClientes", dialogueLinesInicio, callback);
    }

    public IEnumerator GetDialogueBuenaLocalized(System.Action<List<string>> callback)
    {
        yield return GetLocalizedLinesFromTable("DialogosClientes", dialogueLinesBuena, callback);
    }

    public IEnumerator GetDialogueMalaLocalized(System.Action<List<string>> callback)
    {
        yield return GetLocalizedLinesFromTable("DialogosClientes", dialogueLinesMala, callback);
    }

    private IEnumerator GetLocalizedLinesFromTable(string tableName, string[] keys, System.Action<List<string>> callback)
    {
        var results = new List<string>();

        foreach (var key in keys)
        {
            if (string.IsNullOrEmpty(key))
            {
                results.Add("");
                continue;
            }

            var localizedString = new LocalizedString
            {
                TableReference = tableName,
                TableEntryReference = key
            };

            var handle = localizedString.GetLocalizedStringAsync();
            yield return handle;

            results.Add(handle.Result);
        }

        callback(results);
    }

    public IEnumerator GetDescripcionPedidoLocalized(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(descripcionPedido))
        {
            callback("");
            yield break;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "DescripcionPedidos",
            TableEntryReference = descripcionPedido
        };

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;
        callback(handle.Result);
    }

    public IEnumerator GetNombreClienteLocalized(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(nombreDelCliente))
        {
            callback("");
            yield break;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "Clientes",
            TableEntryReference = nombreDelCliente 
        };

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;
        callback(handle.Result);
    }

    private IEnumerator GetLocalizedString(LocalizedString localizedString, System.Action<string> callback)
    {
        if (localizedString == null || string.IsNullOrEmpty(localizedString.TableEntryReference))
        {
            callback("");
            yield break;
        }

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;
        callback(handle.Result);
    }
    #endregion

    #region Stickers y Pistas
    public List<StickerID> stickersRequeridos = new List<StickerID>();

    [Header("Reparación")]
    public List<LocalizedString> pistasReparacion = new List<LocalizedString>();
    public PageCategory categoriaLibroReparar = PageCategory.Default;

    [Header("Portada")]
    public string tituloLibroPortada = "";
    public LocalizedString tituloLibroPortadaKey;
    public StickerSet setStickersDeseado = StickerSet.Default;
    public List<LocalizedString> pistasPortada = new List<LocalizedString>();
    #endregion

    #region Hechizar
    [Header("Hechizo")]
    public Hechizo hechizoSolicitado = Hechizo.Ninguno;
    public LocalizedString tituloLibroHechizadoKey;
    public List<LocalizedString> pistasHechizo = new List<LocalizedString>();
    #endregion

    #region Traducir
    [Header("Traducción")]
    public string claveMensajeCriptograma;
    #endregion

    #region Encanto 
    [Header("Encanto")]
    public TipoEncanto tipoEncantoSolicitado = TipoEncanto.Ninguno;
    #endregion

    #region Trivia
    [Header("Trivia")]
    [SerializeField] private List<Question> m_preguntasTrivia = new List<Question>();
    public List<Question> PreguntasTrivia => m_preguntasTrivia;
    #endregion

    #region Donación y Devolución
    [Header("Donación")]
    public int libroDonadoID;

    [Header("Devolución")]
    public int libroDevueltoID;
    public string tituloLibroDevuelto = "";
    public LocalizedString tituloLibroDevueltoKey;

    [Header("Préstamo")]
    public string tituloLibroPrestado = "";
    public LocalizedString tituloLibroPrestadoKey;
    #endregion

    #region Métodos de Acceso Rápido
    public string[] GetDialogueInicio() => dialogueLinesInicio;
    public string[] GetDialogueBuena() => dialogueLinesBuena;
    public string[] GetDialogueMala() => dialogueLinesMala;

    public IEnumerator GetTituloLibroPortadaLocalized(System.Action<string> callback)
    {
        yield return GetLocalizedString(tituloLibroPortadaKey, callback);
    }

    public IEnumerator GetTituloLibroPrestadoLocalized(System.Action<string> callback)
    {
        yield return GetLocalizedString(tituloLibroPrestadoKey, callback);
    }

    public IEnumerator GetTituloLibroDevueltoLocalized(System.Action<string> callback)
    {
        yield return GetLocalizedString(tituloLibroDevueltoKey, callback);
    }

    public IEnumerator GetTituloLibroHechizadoLocalized(System.Action<string> callback)
    {
        yield return GetLocalizedString(tituloLibroHechizadoKey, callback);
    }
    #endregion
}
