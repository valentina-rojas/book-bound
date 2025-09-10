using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(AudioSource))]
public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    #region Configuración
    [Header("Audio y Colores")]
    [SerializeField] private Color m_correctColor = Color.green;
    [SerializeField] private Color m_incorrectColor = Color.red;
    [SerializeField] private float m_waitTime = 1.0f;
    [SerializeField] private int totalQuestions = 5;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private Button closeButton;

    [Header("Resultados Localizados")]
    [SerializeField] private LocalizeStringEvent correctLabelText;
    [SerializeField] private LocalizeStringEvent incorrectLabelText;

    [Header("Resultados Numéricos")]
    [SerializeField] private TextMeshProUGUI correctCountText;
    [SerializeField] private TextMeshProUGUI incorrectCountText;
    #endregion

    #region Privados
    private QuizDB m_quizDB;
    private QuizUI m_quizUI;
    private AudioSource m_audioSource;

    private int correctCount = 0;
    private int incorrectCount = 0;
    private int currentQuestionIndex = 0;
    private bool isWaiting = false;

    private List<Question> currentQuestions;
    #endregion

    #region Ciclo de Vida
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        m_quizDB = Object.FindFirstObjectByType<QuizDB>();
        m_quizUI = Object.FindFirstObjectByType<QuizUI>();
        m_audioSource = GetComponent<AudioSource>();

        quizPanel.SetActive(false);
        resultsPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseResults);
    }
    #endregion

    #region Manejo del Quiz
    public void StartQuiz(List<Question> preguntasPersonaje)
    {
        correctCount = 0;
        incorrectCount = 0;
        currentQuestionIndex = 0;
        currentQuestions = preguntasPersonaje;
        quizPanel.SetActive(true);
        resultsPanel.SetActive(false);
        CameraManager.instance?.DesactivarBotonCamara();
        NextQuestion();
    }

    private void NextQuestion()
    {
        isWaiting = false;

        if (currentQuestionIndex >= currentQuestions.Count)
        {
            ShowResults();
            return;
        }

        m_quizUI.Construct(currentQuestions[currentQuestionIndex], GiveAnswer);
    }

    private void GiveAnswer(OptionButton optionButton)
    {
        if (isWaiting) return;
        isWaiting = true;

        bool isCorrect = optionButton.Option.correct;

        if (isCorrect)
            correctCount++;
        else
            incorrectCount++;

        StartCoroutine(GiveAnswerRoutine(optionButton));
    }

    private IEnumerator GiveAnswerRoutine(OptionButton optionButton)
    {
        if (m_audioSource.isPlaying)
            m_audioSource.Stop();

        bool isCorrect = optionButton.Option.correct;

        optionButton.SetColor(isCorrect ? m_correctColor : m_incorrectColor);

        if (isCorrect)
            AudioManager.instance.sonidoRespuestaTriviaCorrecta.Play();
        else
            AudioManager.instance.sonidosonidoRespuestaTriviaIncorrecta.Play();

        if (GameManager.instance != null)
            GameManager.instance.ActualizarSpritePorRespuesta(isCorrect);

        yield return new WaitForSeconds(m_waitTime);

        currentQuestionIndex++;

        if (currentQuestionIndex >= totalQuestions)
            ShowResults();
        else
            NextQuestion();
    }

    private void ShowResults()
    {
        quizPanel.SetActive(false);
        resultsPanel.SetActive(true);
        correctCountText.text = correctCount.ToString();
        incorrectCountText.text = incorrectCount.ToString();
    }

    private void CloseResults()
    {
        resultsPanel.SetActive(false);
        CameraManager.instance?.ActivarBotonCamara();
        if (GameManager.instance != null)
            GameManager.instance.CompletarTrivia(correctCount, incorrectCount);
    }
    #endregion
}