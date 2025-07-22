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

    public Sprite spriteRespuestaBuena;
    public Sprite spriteRespuestaMala;

    public int libroDeseadoID;
    public string tipoPreferido;
    public string tituloLibroPrestado = "";

    public string nombreDelCliente;
    [TextArea(1, 3)]
    public string descripcionPedido;

    public List<StickerID> stickersRequeridos = new List<StickerID>();
    public string tituloLibroPortada = "";
    public Hechizo hechizoSolicitado = Hechizo.Ninguno;

    public int libroDonadoID;

    public int libroDevueltoID;
    public string tituloLibroDevuelto = "";

    public string[] GetDialogueInicio() => dialogueLinesInicio;
    public string[] GetDialogueBuena() => dialogueLinesBuena;
    public string[] GetDialogueMala() => dialogueLinesMala;

    public IEnumerator GetTituloLibroPortadaLocalized(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(tituloLibroPortada))
        {
            callback("");
            yield break;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "TitulosLibrosPortada",  
            TableEntryReference = tituloLibroPortada
        };

        var handle = localizedString.GetLocalizedStringAsync();

        yield return handle;

        callback(handle.Result);
    }
}