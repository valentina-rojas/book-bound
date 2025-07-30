using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

public class CharacterAttributes : MonoBehaviour
{
    public enum TipoDePedido
    {
        BuscarLibro,
        RepararLibro,
        HacerPortada,
        HechizarLibro,
        JuegoTrivia,
        DonarLibro,
        DevolverLibro 
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

    public TipoDePedido tipoDePedido;

    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesInicio;
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesBuena;
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesMala;

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

    public Sprite spriteRespuestaBuena;
    public Sprite spriteRespuestaMala;

    public int libroDeseadoID;
    public string tipoPreferido;

    public string nombreDelCliente;

    [Tooltip("Clave de la tabla 'DescripcionPedidos'")]
    public string descripcionPedido; 

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

    public List<StickerID> stickersRequeridos = new List<StickerID>();

    [Header("Portada")]
    public string tituloLibroPortada = "";
    public LocalizedString tituloLibroPortadaKey;

    [Header("Hechizo")]
    public Hechizo hechizoSolicitado = Hechizo.Ninguno;
    public LocalizedString tituloLibroHechizadoKey;

    [Header("Donación")]
    public int libroDonadoID;

    [Header("Devolución")]
    public int libroDevueltoID;
    public string tituloLibroDevuelto = "";
    public LocalizedString tituloLibroDevueltoKey;

    [Header("Préstamo")]
    public string tituloLibroPrestado = "";
    public LocalizedString tituloLibroPrestadoKey;

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
}