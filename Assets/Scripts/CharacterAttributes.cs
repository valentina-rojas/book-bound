using UnityEngine;

public class CharacterAttributes : MonoBehaviour
{

 [Header("Diálogos iniciales")]
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesInicio;

    [Header("Diálogos si la recomendación fue buena")]
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesBuena;

    [Header("Diálogos si la recomendación fue mala")]
    [SerializeField, TextArea(2, 4)] private string[] dialogueLinesMala;

  [Header("Preferencias del personaje")]
  public int libroDeseadoID;
  public string tipoPreferido;



    public string[] GetDialogueInicio() => dialogueLinesInicio;
    public string[] GetDialogueBuena() => dialogueLinesBuena;
    public string[] GetDialogueMala() => dialogueLinesMala;
}
