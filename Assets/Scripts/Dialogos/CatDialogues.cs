using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CatDialogues : MonoBehaviour
{
    #region UI
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private Button botonRepetir;
    [SerializeField] private Button botonFinalizar;
    #endregion

    #region Configuración
    private float typingTime = 0.05f;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private int lineIndex;
    private string[] dialogueKeys;
    private int diaActual;
    private bool esDialogoExtra = false;
    private string tablaActual = "CatDialogue"; 
    private bool cancelandoDialogo = false;

    #endregion

    #region Diálogos por día
    private Dictionary<int, string[]> dialoguesPorDia = new Dictionary<int, string[]>
    {
        { 1, new string[] { "cat1_1", "cat1_2", "cat1_3", "cat1_4", "cat1_5", "cat1_6" } },
        { 2, new string[] { "cat2_1", "cat2_2", "cat2_3", "cat2_4", "cat2_5" } },
        { 3, new string[] { "cat3_1", "cat3_2", "cat3_3", "cat3_4" } },
        { 4, new string[] { "cat4_1", "cat4_2", "cat4_3", "cat4_4", "cat4_5", "cat4_6" } },
        { 5, new string[] { "cat5_1", "cat5_2", "cat5_3", "cat5_4", "cat5_5" } },
        { 6, new string[] { "cat6_1", "cat6_2", "cat6_3", "cat6_4" } },
        { 7, new string[] { "cat7_1", "cat7_2", "cat7_3", "cat7_4", "cat7_5" } }
    };
    #endregion

    #region Inicialización
    void Start()
    {
        if (botonSiguiente != null)
        {
            botonSiguiente.onClick.AddListener(OnBotonSiguienteClick);
            botonSiguiente.gameObject.SetActive(false);
        }

        if (botonRepetir != null)
        {
            botonRepetir.onClick.AddListener(OnBotonRepetirClick);
            botonRepetir.gameObject.SetActive(false);
        }

        if (botonFinalizar != null)
        {
            botonFinalizar.onClick.AddListener(FinalizarDialogo);
            botonFinalizar.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Lógica del diálogo

    public void IniciarDialogoDelDia(int dia)
    {
        if (dialoguesPorDia.ContainsKey(dia))
        {
            diaActual = dia;
            dialogueKeys = dialoguesPorDia[dia];
            esDialogoExtra = false;
            tablaActual = "CatDialogue"; 
            StartDialogue();
        }
    }

    public void IniciarDialogoExtra(string mensaje)
    {
        IniciarDialogoExtraDesdeLista(new string[] { mensaje });
    }

    public void IniciarDialogoExtraDesdeLista(string[] mensajes, string tabla = "Extra")
    {
        dialogueKeys = mensajes;
        esDialogoExtra = true;
        tablaActual = tabla;
        StartDialogue();
    }

    private void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        lineIndex = 0;

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);

        ActualizarTextoBoton();
        typingCoroutine = StartCoroutine(ShowLocalizedLine());
    }

    public System.Action OnDialogoUltimaLineaTipeada; 

    private IEnumerator ShowLocalizedLine()
    {
        isTyping = true;
        dialogueText.text = string.Empty;
        cancelandoDialogo = false;

        string key = dialogueKeys[lineIndex];
        LocalizedString localizedString = new LocalizedString(tablaActual, key);

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;

        if (cancelandoDialogo) yield break;

        string line = handle.Result;

        foreach (char ch in line)
        {
            if (cancelandoDialogo) yield break;
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        if (cancelandoDialogo) yield break;

        isTyping = false;
        ActualizarTextoBoton();

        if (lineIndex == dialogueKeys.Length - 1)
        {
            OnDialogoUltimaLineaTipeada?.Invoke();
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        if (lineIndex < dialogueKeys.Length)
        {
            typingCoroutine = StartCoroutine(ShowLocalizedLine());
        }
        else
        {
            lineIndex = dialogueKeys.Length - 1;

            if (botonFinalizar != null)
                botonFinalizar.gameObject.SetActive(true);

            if (botonRepetir != null)
                botonRepetir.gameObject.SetActive(true);
        }
    }

    public System.Action OnDialogoExtraFinalizado;
    public void FinalizarDialogo()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        StopAllCoroutines(); 

        isTyping = false;
        lineIndex = 0;
        dialogueText.text = string.Empty;

        dialoguePanel.SetActive(false);
        botonSiguiente?.gameObject.SetActive(false);
        botonFinalizar?.gameObject.SetActive(false);
        botonRepetir?.gameObject.SetActive(false);

        if (!esDialogoExtra)
        {
            if (lineIndex == dialogueKeys.Length - 1)
                OnDialogoUltimaLineaTipeada?.Invoke();

            TaskManager.instance?.MostrarTareas();

            if (diaActual == 1)
                Tutorial.instance?.EmpezarTutorial();
        }
        else
        {
            OnDialogoExtraFinalizado?.Invoke();
        }
    }

    public void CancelarDialogo()
    {
        cancelandoDialogo = true;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        StopAllCoroutines();

        dialogueText.text = string.Empty;
        dialoguePanel.SetActive(false);
        botonSiguiente?.gameObject.SetActive(false);
        botonFinalizar?.gameObject.SetActive(false);
        botonRepetir?.gameObject.SetActive(false);

        isTyping = false;
        lineIndex = 0;
    }

    private void ActualizarTextoBoton()
    {
        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);
    }
    #endregion

    #region Eventos de botones

    private void OnBotonSiguienteClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            LocalizedString temp = new LocalizedString(tablaActual, dialogueKeys[lineIndex]);
            temp.GetLocalizedStringAsync().Completed += handle =>
            {
                dialogueText.text = handle.Result;
                isTyping = false;
                ActualizarTextoBoton();
            };
        }
        else
        {
            NextDialogueLine();
        }
    }

    private void OnBotonRepetirClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        StartDialogue();
    }

    #endregion
}