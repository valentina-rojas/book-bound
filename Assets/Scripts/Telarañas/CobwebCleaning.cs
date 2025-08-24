using UnityEngine;

public class CobwebCleaning : MonoBehaviour
{
    public float cantidadClicsParaDesaparecer = 5f;
    private float clicsActuales = 0f;
    private SpriteRenderer sr;

    [Header("¿Es la telaraña del tutorial?")]
    public bool esTelarañaTutorial = false;

    [Header("Nivel mínimo para que aparezca esta telaraña")]
    public int nivelMinimo = 1;

    [Header("Sala a la que pertenece")]
    public string sala; 

    [HideInInspector] public bool puedeInteractuar = true;
    [HideInInspector] public bool interaccionFueHabilitada = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        CobwebManager.instance.RegistrarTelaraña(this);

        if (esTelarañaTutorial)
            puedeInteractuar = false;
    }

    private void OnMouseDown()
    {
        if (!puedeInteractuar) return;

        clicsActuales++;
        float alpha = Mathf.Lerp(1f, 0f, clicsActuales / cantidadClicsParaDesaparecer);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (clicsActuales >= cantidadClicsParaDesaparecer)
        {
            CobwebManager.instance.EliminarTelaraña(this);
            gameObject.SetActive(false);

            if (esTelarañaTutorial)
                Tutorial.instance?.AvanzarAlSiguientePaso();
        }
    }

    public void ReiniciarTelaraña()
    {
        clicsActuales = 0f;
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
