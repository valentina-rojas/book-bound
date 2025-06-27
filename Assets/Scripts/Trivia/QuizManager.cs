using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;


    [Header("Audio y Colores")]
    [SerializeField] private Color m_correctColor = Color.green;
    [SerializeField] private Color m_incorrectColor = Color.red;
    [SerializeField] private float m_waitTime = 1.0f;
    [SerializeField] private int totalQuestions = 5;
    
    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private Button closeButton;


    private QuizDB m_quizDB = null;
    private QuizUI m_quizUI = null;
    private AudioSource m_audioSource = null;

    private int correctCount = 0;
    private int incorrectCount = 0;
    private int currentQuestionIndex = 0;
    private bool isWaiting = false;


     private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Opcional: si quieres que persista entre escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


     private void Start()
    {
        m_quizDB = FindObjectOfType<QuizDB>();
        m_quizUI = FindObjectOfType<QuizUI>();
        m_audioSource = GetComponent<AudioSource>();

        quizPanel.SetActive(false);
        resultsPanel.SetActive(false);

        closeButton.onClick.AddListener(CloseResults);

        // llamar a StartQuiz() luego de los dialogos del personaje que correspondan
    }

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

        if (optionButton.Option.correct)
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
        {
            AudioManager.instance.sonidoRespuestaTriviaCorrecta.Play();
        }
        else
        {
            AudioManager.instance.sonidosonidoRespuestaTriviaIncorrecta.Play();
        }


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
        //llamar a dialogo de personaje o funcion en gamemanager

        if (GameManager.instance != null)
        {
        GameManager.instance.CompletarTrivia(correctCount, incorrectCount);
        }

       
       
    }
}