using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    #region UI
    [SerializeField] private Button dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    private Button botonSiguiente;
    private TMP_Text botonSiguienteTexto;
    private Button botonRepetir;
    private Button botonFinalizar;
    #endregion

    #region Configuración
    private float typingTime = 0.05f;
    private bool isMouseOver = false;
    private bool didDialogueStart;
    private int lineIndex;
    private bool hasInteracted = false;
    private string[] dialogueLines;
    private CharacterAttributes characterAttributes;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    #endregion

    #region Inicialización
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

        botonSiguiente?.onClick.AddListener(NextDialogueLine);
        botonRepetir?.onClick.AddListener(ReiniciarDialogo);
        botonFinalizar?.onClick.AddListener(FinalizarDialogo);
        dialogueMark?.onClick.AddListener(EmpezarDialogoResultado);

        characterAttributes = GetComponent<CharacterAttributes>();

        botonSiguiente?.gameObject.SetActive(false);
        botonRepetir?.gameObject.SetActive(false);
        botonFinalizar?.gameObject.SetActive(false);
        dialogueMark?.gameObject.SetActive(false);
    }
    #endregion

    #region Lógica del diálogo
    public void EmpezarDialogoResultado()
    {
        hasInteracted = false;
        StartDialogue();
    }

    private void StartDialogue()
    {
        if (characterAttributes == null) return;

        TaskManager.instance.OcultarListaTareas();
        dialoguePanel.SetActive(true);
        dialogueMark.gameObject.SetActive(false);

        didDialogueStart = true; 

        switch (GameManager.instance.resultadoRecomendacion)
        {
            case GameManager.ResultadoRecomendacion.Buena:
                StartCoroutine(characterAttributes.GetDialogueBuenaLocalized(OnDialoguesReady));
                break;
            case GameManager.ResultadoRecomendacion.Mala:
                StartCoroutine(characterAttributes.GetDialogueMalaLocalized(OnDialoguesReady));
                break;
            default:
                StartCoroutine(characterAttributes.GetDialogueInicioLocalized(OnDialoguesReady));
                break;
        }
    }

    private void OnDialoguesReady(List<string> localizedLines)
    {
        if (localizedLines == null || localizedLines.Count == 0)
        {
            Debug.LogWarning("No se recibieron líneas localizadas.");
            return;
        }

        dialogueLines = localizedLines.ToArray();
        lineIndex = 0;
        hasInteracted = false;

        botonSiguiente?.gameObject.SetActive(true);
        botonRepetir?.gameObject.SetActive(false);
        botonFinalizar?.gameObject.SetActive(false);

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
            botonFinalizar?.gameObject.SetActive(true);
            botonRepetir?.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowLine()
    {
        isTyping = true;
        dialogueText.text = "";

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
        botonSiguiente?.gameObject.SetActive(true);
    }

    private void ReiniciarDialogo()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[lineIndex];
            isTyping = false;
            return;
        }

        if (characterAttributes == null) return;

        TaskManager.instance.OcultarListaTareas();
        dialoguePanel.SetActive(true);
        dialogueMark.gameObject.SetActive(false);

        switch (GameManager.instance.resultadoRecomendacion)
        {
            case GameManager.ResultadoRecomendacion.Buena:
                StartCoroutine(characterAttributes.GetDialogueBuenaLocalized(OnDialoguesReady));
                break;
            case GameManager.ResultadoRecomendacion.Mala:
                StartCoroutine(characterAttributes.GetDialogueMalaLocalized(OnDialoguesReady));
                break;
            default:
                StartCoroutine(characterAttributes.GetDialogueInicioLocalized(OnDialoguesReady));
                break;
        }
    }

    private void FinalizarDialogo()
    {
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        dialogueMark.gameObject.SetActive(false);
        hasInteracted = true;

        botonSiguiente?.gameObject.SetActive(false);
        botonRepetir?.gameObject.SetActive(false);
        botonFinalizar?.gameObject.SetActive(false);

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
            dialogueMark?.gameObject.SetActive(true);
        }
    }

    public bool HaTerminadoElDialogo()
    {
        return !didDialogueStart;
    }
    #endregion
}