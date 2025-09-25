using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    public int dineroInicial = 0;
    private int dineroActual;

    public TMP_Text textoDinero;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dineroActual = dineroInicial;
        ActualizarUI();
    }

    public void SumarDinero(int cantidad)
    {
        if (cantidad < 0) return;

        dineroActual += cantidad;
        ActualizarUI();
    }

    public void RestarDinero(int cantidad)
    {
        if (cantidad < 0) return;

        dineroActual -= cantidad;
        if (dineroActual < 0) dineroActual = 0;
        ActualizarUI();
    }

    public int ObtenerDinero()
    {
        return dineroActual;
    }

    public void ReiniciarDinero()
    {
        dineroActual = dineroInicial;
        ActualizarUI();
    }

    public void EstablecerDinero(int cantidad)
    {
        dineroActual = cantidad;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoDinero != null)
            textoDinero.text = $"{dineroActual}";
    }
}
