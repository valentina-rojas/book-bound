using UnityEngine;
using UnityEngine.UI;

public class TendCat : MonoBehaviour
{
    public static TendCat instance;

    [Header("GameObjects de interacción")]
    public GameObject cepilloGO;
    public GameObject bolsaComidaGO;
    public GameObject platitoGO;

    #region Cepillado
    public RectTransform cepilloUI;
    public RectTransform areaCepilladoUI;
    public float tiempoNecesario = 2f;
    public Slider barraCepilladoUI;

    private Vector2 ultimaPosicionCepillo;
    private bool estaMoviendose = false;
    private float tiempoSobreAreaCepillado = 0f;
    private bool tareaCepillarCompletada = false;
    #endregion

    #region Alimentar
    public RectTransform bolsaComidaUI;
    public RectTransform platitoUI;
    public Sprite platitoLlenoSprite;
    public Sprite platitoVacioSprite;

    private bool tareaAlimentarCompletada = false;
    #endregion

    #region Acariciar
    public GameObject corazonesGO;
    #endregion

    private Camera camara;

    #region Unity
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        camara = Camera.main;
        if (barraCepilladoUI != null) barraCepilladoUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        VerificarCepillado();
        VerificarAlimentacion();
        VerificarAcariciar();
    }
    #endregion

    #region Actualización UI
    public void ActualizarVisibilidadObjetos()
    {
        if (TaskManager.instance == null) return;

        if (cepilloGO != null) cepilloGO.SetActive(TaskManager.instance.EsTareaActiva(2));

        bool mostrarComida = TaskManager.instance.EsTareaActiva(3);
        if (bolsaComidaGO != null) bolsaComidaGO.SetActive(mostrarComida);
        if (platitoGO != null) platitoGO.SetActive(mostrarComida);
    }
    #endregion

    #region Cepillado
    private void VerificarCepillado()
    {
        if (tareaCepillarCompletada) return;

        Vector2 posicionCepillo = RectTransformUtility.WorldToScreenPoint(camara, cepilloUI.position);
        estaMoviendose = (Vector2.Distance(posicionCepillo, ultimaPosicionCepillo) > 0.5f);
        ultimaPosicionCepillo = posicionCepillo;

        if (RectTransformUtility.RectangleContainsScreenPoint(areaCepilladoUI, posicionCepillo, camara) && estaMoviendose)
        {
            if (barraCepilladoUI != null && !barraCepilladoUI.gameObject.activeSelf)
                barraCepilladoUI.gameObject.SetActive(true);

            tiempoSobreAreaCepillado += Time.deltaTime;

            if (barraCepilladoUI != null)
                barraCepilladoUI.value = tiempoSobreAreaCepillado / tiempoNecesario;

            if (tiempoSobreAreaCepillado >= tiempoNecesario)
            {
                tareaCepillarCompletada = true;

                if (barraCepilladoUI != null)
                {
                    barraCepilladoUI.value = 1f;
                    barraCepilladoUI.gameObject.SetActive(false);
                }

                AudioManager.instance.sonidoGato.Play();
                TaskManager.instance.CompletarTareaPorID(2);
            }
        }
    }

    public void ReiniciarBarraCepillado()
    {
        tiempoSobreAreaCepillado = 0f;
        if (barraCepilladoUI != null)
        {
            barraCepilladoUI.value = 0f;
            barraCepilladoUI.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Alimentar
    private void VerificarAlimentacion()
    {
        if (tareaAlimentarCompletada) return;

        Rect rectPlatito = GetScreenRect(platitoUI);
        Rect rectBolsa = GetScreenRect(bolsaComidaUI);

        if (rectPlatito.Overlaps(rectBolsa))
        {
            tareaAlimentarCompletada = true;

            AudioManager.instance.sonidoGato.Play();
            TaskManager.instance.CompletarTareaPorID(3);

            Image platitoImage = platitoUI.GetComponent<Image>();
            if (platitoImage != null && platitoLlenoSprite != null)
                platitoImage.sprite = platitoLlenoSprite;
        }
    }
    #endregion

    #region Acariciar
    private void VerificarAcariciar()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            if (RectTransformUtility.RectangleContainsScreenPoint(areaCepilladoUI, mousePos, camara))
            {
                AudioManager.instance.sonidoRonroneo.Play();

                if (corazonesGO != null)
                {
                    corazonesGO.SetActive(true);
                    Animator animator = corazonesGO.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.enabled = false;
                        animator.enabled = true;
                        animator.Play("corazones", -1, 0f);
                    }
                }
            }
        }

        if (corazonesGO != null && !AudioManager.instance.sonidoRonroneo.isPlaying)
            corazonesGO.SetActive(false);
    }
    #endregion

    #region Utilidades
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
        ReiniciarBarraCepillado();
        tareaAlimentarCompletada = false;

        Image platitoImage = platitoUI.GetComponent<Image>();
        if (platitoImage != null && platitoVacioSprite != null)
            platitoImage.sprite = platitoVacioSprite;
    }
    #endregion
}