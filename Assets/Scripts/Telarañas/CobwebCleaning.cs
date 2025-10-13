using UnityEngine;

public class CobwebCleaning : MonoBehaviour
{
    [Header("Configuración de limpieza")]
    public float cantidadClicsParaDesaparecer = 5f; 
    private float progresoLimpieza = 0f;
    private SpriteRenderer sr;

    [Header("¿Es la telaraña del tutorial?")]
    public bool esTelarañaTutorial = false;

    [Header("Nivel mínimo para que aparezca esta telaraña")]
    public int nivelMinimo = 1;

    [Header("Sala a la que pertenece")]
    public string sala;

    [HideInInspector] public bool puedeInteractuar = true;
    [HideInInspector] public bool interaccionFueHabilitada = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CobwebManager.instance.RegistrarTelaraña(this);

        if (esTelarañaTutorial)
            puedeInteractuar = false;
    }

    public void LimpiarTick(float delta)
    {
        if (!puedeInteractuar) return;

        progresoLimpieza += delta;

        float alpha = Mathf.Lerp(1f, 0f, progresoLimpieza / cantidadClicsParaDesaparecer);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (progresoLimpieza >= cantidadClicsParaDesaparecer)
        {
            CobwebManager.instance.EliminarTelaraña(this);
            gameObject.SetActive(false);

            if (esTelarañaTutorial && Tutorial.instance != null && !Tutorial.instance.tutorialSaltado)
            {
                Tutorial.instance.AvanzarAlSiguientePaso();
            }
        }
    }

    public void ReiniciarTelaraña()
    {
        progresoLimpieza = 0f;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        gameObject.SetActive(true);

        if (esTelarañaTutorial && !interaccionFueHabilitada)
            puedeInteractuar = false;
        else
            puedeInteractuar = true;
    }

    public void HabilitarInteraccion()
    {
        puedeInteractuar = true;
        interaccionFueHabilitada = true;
    }
}