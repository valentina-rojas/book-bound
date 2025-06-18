using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class CatDialogues : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private Button botonRepetir;
    [SerializeField] private Button botonFinalizar;

    private float typingTime = 0.05f;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private int lineIndex;
    private string[] dialogueLines;
    private int diaActual;
    private bool esDialogoExtra = false;

    private Dictionary<int, string[]> dialoguesPorDia = new Dictionary<int, string[]>()
    {
        { 1, new string[] {
            "¡Hola! Soy Minino, tu asistente en esta librería mágica.",
            "Cada día vendrán criaturas distintas buscando libros.",
            "Recomiéndales libros según sus gustos o el que buscan exactamente.",
            "Pero antes de recibir clientes deberías limpiar un poco este lugar...",
            "¡Suerte en tu primer día!" }
        },
        { 2, new string[] {
            "¡Buen trabajo ayer!",
            "Hoy los clientes serán un poco más exigentes ¡Y yo igual!.",
            "No puedes dejarme sin comer ni cepillarme.",
            "Y deberías cuidar las plantas del patio o Rhea se pondra triste cuando vuelva...",
            "Pero por el resto vas bien ¡Sigue así!."
        }},
        { 3, new string[] {
            "¡Ya eres todo un experto!",
            "Puede que los clientes cada vez hagan pedidos más diferentes...",
            "¡Pero confía en tu instinto de librero mágico!",
            "¡Vas muy bien!"
        }},
        { 4, new string[] {
            "Sin duda Rhea eligió a la persona correcta para cuidar este lugar.",
            "Así que creo que podríamos abrir la sala de lectura a partir de hoy.",
            "No te preocupes, casi ni notarás que está abierta.",
            "¡Buena suerte!"
        }},
        { 5, new string[] {
            "¡Vas muy bien!"
        }},
        { 6, new string[] {
            "¡Vas muy bien!"
        }},
        { 7, new string[] {
            "¡Vas muy bien!"
        }}
    };

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

    private void OnBotonSiguienteClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[lineIndex];
            isTyping = false;
            ActualizarTextoBoton();
        }
        else
        {
            NextDialogueLine();
        }
    }

    private void OnBotonRepetirClick()
    {
        StartDialogue();
    }

    public void IniciarDialogoDelDia(int dia)
    {
        if (dialoguesPorDia.ContainsKey(dia))
        {
            diaActual = dia;
            dialogueLines = dialoguesPorDia[dia];
            esDialogoExtra = false; 
            StartDialogue();
        }
    }

    public void IniciarDialogoExtra(string mensaje)
    {
        dialogueLines = new string[] { mensaje };
        esDialogoExtra = true; 
        StartDialogue();
    }

    private void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        lineIndex = 0;

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);

        if (botonRepetir != null)
            botonRepetir.gameObject.SetActive(false);

        ActualizarTextoBoton();
        typingCoroutine = StartCoroutine(ShowLine());
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

    private void NextDialogueLine()
    {
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
    public void FinalizarDialogo()
    {
        dialoguePanel.SetActive(false);

        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(false);

        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false);

        if (botonRepetir != null)
            botonRepetir.gameObject.SetActive(false);

        CameraManager.instance?.ActivarBotonCamara();

        if (!esDialogoExtra)
        {
            TaskManager.instance?.MostrarTareas();
        }
    }

   private void ActualizarTextoBoton()
   {
        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(true);

        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false);

        if (botonRepetir != null)
            botonRepetir.gameObject.SetActive(false);
    }

}