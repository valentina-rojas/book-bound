using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Button closeButton;
    #endregion

    #region Privados
    private QuizDB m_quizDB;
    private QuizUI m_quizUI;
    private AudioSource m_audioSource;

    private int correctCount = 0;
    private int incorrectCount = 0;
    private int currentQuestionIndex = 0;
    private bool isWaiting = false;
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
        m_quizDB = FindObjectOfType<QuizDB>();
        m_quizUI = FindObjectOfType<QuizUI>();
        m_audioSource = GetComponent<AudioSource>();

        quizPanel.SetActive(false);
        resultsPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseResults);
    }
    #endregion

    #region Manejo del Quiz
    public void StartQuiz()
    {
        correctCount = 0;
        incorrectCount = 0;
        currentQuestionIndex = 0;

        quizPanel.SetActive(true);
        resultsPanel.SetActive(false);

        NextQuestion();
    }

    private void NextQuestion()
    {
        isWaiting = false;
        m_quizUI.Construct(m_quizDB.GetRandom(), GiveAnswer);
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

        resultsText.text = $" Correctas: {correctCount}\n Incorrectas: {incorrectCount}";
    }

    private void CloseResults()
    {
        resultsPanel.SetActive(false);

        if (GameManager.instance != null)
            GameManager.instance.CompletarTrivia(correctCount, incorrectCount);
    }
    #endregion
}