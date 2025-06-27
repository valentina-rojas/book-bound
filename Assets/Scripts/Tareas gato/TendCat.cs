using UnityEngine;
using UnityEngine.UI;

public class TendCat : MonoBehaviour
{
    public static TendCat instance;

    [Header("GameObjects de interacción")]
    public GameObject cepilloGO;
    public GameObject bolsaComidaGO;
    public GameObject platitoGO;

    [Header("Cepillado")]
    public RectTransform cepilloUI;
    public RectTransform areaCepilladoUI;
    public float tiempoNecesario = 2f;
    public Slider barraCepilladoUI;


    [Header("Alimentar")]
    public RectTransform bolsaComidaUI;
    public RectTransform platitoUI;
    public Sprite platitoLlenoSprite;
    public Sprite platitoVacioSprite;

    private float tiempoSobreAreaCepillado = 0f;
    private bool tareaCepillarCompletada = false;
    private bool tareaAlimentarCompletada = false;

    private Camera camara;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); 
    }
    private void Start()
    {
        camara = Camera.main;
        if (barraCepilladoUI != null)
            barraCepilladoUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        VerificarCepillado();
        VerificarAlimentacion();
    }

    public void ActualizarVisibilidadObjetos()
    {
        if (TaskManager.instance == null)
            return;

        // Mostrar el cepillo si la tarea 2 está activa
        bool mostrarCepillo = TaskManager.instance.EsTareaActiva(2);
        if (cepilloGO != null) cepilloGO.SetActive(mostrarCepillo);

        // Mostrar comida y platito si la tarea 3 está activa
        bool mostrarComida = TaskManager.instance.EsTareaActiva(3);
        if (bolsaComidaGO != null) bolsaComidaGO.SetActive(mostrarComida);
        if (platitoGO != null) platitoGO.SetActive(mostrarComida);
    }

    private void VerificarCepillado()
    {
        if (tareaCepillarCompletada) return;

        Vector2 posicionCepillo = RectTransformUtility.WorldToScreenPoint(camara, cepilloUI.position);

        if (RectTransformUtility.RectangleContainsScreenPoint(areaCepilladoUI, posicionCepillo, camara))
        {
            if (barraCepilladoUI != null && !barraCepilladoUI.gameObject.activeSelf)
                barraCepilladoUI.gameObject.SetActive(true);

            tiempoSobreAreaCepillado += Time.deltaTime;

            if (barraCepilladoUI != null)
                barraCepilladoUI.value = tiempoSobreAreaCepillado / tiempoNecesario;

            Debug.Log($"Cepillando al gato: {tiempoSobreAreaCepillado:F2}s");

            if (tiempoSobreAreaCepillado >= tiempoNecesario)
            {
                tareaCepillarCompletada = true;
                Debug.Log("Gato cepillado correctamente");

                if (barraCepilladoUI != null)
                {
                    barraCepilladoUI.value = 1f;
                    barraCepilladoUI.gameObject.SetActive(false);
                }

                AudioManager.instance.sonidoGato.Play();


                TaskManager.instance.CompletarTareaPorID(2);
            }
        }
        else
        {
            tiempoSobreAreaCepillado = 0f;

            if (barraCepilladoUI != null)
            {
                barraCepilladoUI.value = 0f;
                barraCepilladoUI.gameObject.SetActive(false);
            }
        }
    }

    private void VerificarAlimentacion()
    {
        if (tareaAlimentarCompletada) return;

        Rect rectPlatito = GetScreenRect(platitoUI);
        Rect rectBolsa = GetScreenRect(bolsaComidaUI);

        if (rectPlatito.Overlaps(rectBolsa))
        {
            tareaAlimentarCompletada = true;
            Debug.Log("Gato alimentado correctamente (detectado por Overlaps)");

            AudioManager.instance.sonidoGato.Play();


            TaskManager.instance.CompletarTareaPorID(3);

            Image platitoImage = platitoUI.GetComponent<Image>();
            if (platitoImage != null && platitoLlenoSprite != null)
            {
                platitoImage.sprite = platitoLlenoSprite;
            }
        }
    }

    private Rect GetScreenRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(camara, corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(camara, corners[2]);

        return new Rect(bottomLeft, topRight - bottomLeft);
    }
    
    public void ReiniciarEstado()
    {
        tareaCepillarCompletada = false;
        tiempoSobreAreaCepillado = 0f;

        if (barraCepilladoUI != null)
        {
            barraCepilladoUI.value = 0f;
            barraCepilladoUI.gameObject.SetActive(false);
        }

        tareaAlimentarCompletada = false;

        Image platitoImage = platitoUI.GetComponent<Image>();
        if (platitoImage != null && platitoVacioSprite != null)
        {
            platitoImage.sprite = platitoVacioSprite;
        }
    }
}