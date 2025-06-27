using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Button dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    private Button botonSiguiente;
    private TMP_Text botonSiguienteTexto;
    private Button botonRepetir;
    private Button botonFinalizar;

    private float typingTime = 0.05f;
    private bool isMouseOver = false;
    private bool didDialogueStart;
    private int lineIndex;
    private bool hasInteracted = false;

    private string[] dialogueLines;
    private CharacterAttributes characterAttributes;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
        {
            dialogueMark = uiManager.GetDialogueMark();
            dialoguePanel = uiManager.GetDialoguePanel();
            dialogueText = uiManager.GetDialogueText();
            botonSiguiente = uiManager.GetBotonSiguiente();
            botonSiguienteTexto = uiManager.GetBotonSiguienteTexto();
            botonRepetir = uiManager.GetBotonRepetir();
            botonFinalizar = uiManager.GetBotonFinalizar();
        }
        else
        {
            Debug.LogError("UIManager no encontrado en la escena.");
        }

        if (botonSiguiente != null)
        {
            botonSiguiente.onClick.AddListener(NextDialogueLine);
            botonSiguiente.gameObject.SetActive(false);
        }

        if (botonRepetir != null)
        {
            botonRepetir.onClick.AddListener(ReiniciarDialogo);
            botonRepetir.gameObject.SetActive(false);
        }

        if (botonFinalizar != null)
        {
            botonFinalizar.onClick.AddListener(FinalizarDialogo);
            botonFinalizar.gameObject.SetActive(false);
        }

        if (dialogueMark != null)
        {
            dialogueMark.onClick.AddListener(EmpezarDialogoResultado);
            dialogueMark.gameObject.SetActive(false);
        }

        characterAttributes = GetComponent<CharacterAttributes>();
    }

    public void EmpezarDialogoResultado()
    {
        hasInteracted = false;
        StartDialogue();
    }

    private void StartDialogue()
    {
        if (characterAttributes == null) return;

        switch (GameManager.instance.resultadoRecomendacion)
        {
            case GameManager.ResultadoRecomendacion.Buena:
                dialogueLines = characterAttributes.GetDialogueBuena();
                break;
            case GameManager.ResultadoRecomendacion.Mala:
                dialogueLines = characterAttributes.GetDialogueMala();
                break;
            default:
                dialogueLines = characterAttributes.GetDialogueInicio();
                break;
        }

        if (dialogueLines == null || dialogueLines.Length == 0) return;

        didDialogueStart = true;
        TaskManager.instance.OcultarListaTareas();
        dialoguePanel.SetActive(true);
        dialogueMark.gameObject.SetActive(false);
        lineIndex = 0;

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);
        if (botonRepetir != null)
            botonRepetir.gameObject.SetActive(false);

        typingCoroutine = StartCoroutine(ShowLine());
    }

    public void NextDialogueLine()
    {
        if (!didDialogueStart || dialogueLines == null) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[lineIndex];
            isTyping = false;
            return;
        }

        lineIndex++;

        if (lineIndex < dialogueLines.Length)
        {
            typingCoroutine = StartCoroutine(ShowLine());
        }
        else
        {
            lineIndex = dialogueLines.Length - 1; 
            botonFinalizar.gameObject.SetActive(true);
            botonRepetir.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowLine()
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;
        ActualizarTextoBoton();
    }

    private void ActualizarTextoBoton()
    {
        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);
    }

    private void ReiniciarDialogo()
    {
        if (characterAttributes == null) return;

        switch (GameManager.instance.resultadoRecomendacion)
        {
            case GameManager.ResultadoRecomendacion.Buena:
                dialogueLines = characterAttributes.GetDialogueBuena();
                break;
            case GameManager.ResultadoRecomendacion.Mala:
                dialogueLines = characterAttributes.GetDialogueMala();
                break;
            default:
                dialogueLines = characterAttributes.GetDialogueInicio();
                break;
        }

        if (dialogueLines == null || dialogueLines.Length == 0) return;

        lineIndex = 0;
        hasInteracted = false;
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.gameObject.SetActive(false);

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);

        ActualizarTextoBoton();
        typingCoroutine = StartCoroutine(ShowLine());
    }

    private void FinalizarDialogo()
    {
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        dialogueMark.gameObject.SetActive(false);
        hasInteracted = true;

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(false);
        if (botonRepetir != null)
            botonRepetir.gameObject.SetActive(false);
        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false);

        CharacterManager characterManager = FindObjectsByType<CharacterManager>(FindObjectsSortMode.None)[0];
        if (characterManager != null && characterAttributes != null)
        {
            characterManager.AtenderPersonaje(characterAttributes);
        }
    }

    public void EnableDialogue()
    {
        if (!hasInteracted)
        {
            isMouseOver = true;
            dialogueMark.gameObject.SetActive(true);
        }
    }

    public bool HaTerminadoElDialogo()
    {
        return !didDialogueStart;
    }
}