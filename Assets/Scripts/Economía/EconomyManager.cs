using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    public int dineroInicial = 0;
    private int dineroActual;
    private int dineroInicioNivel; 

    public TMP_Text textoDinero;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        dineroActual = dineroInicial;
        dineroInicioNivel = dineroActual; 
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

    public int ObtenerDinero() => dineroActual;

    public void FijarDineroInicioNivel()
    {
        dineroInicioNivel = dineroActual;
    }

    public void ReiniciarDineroNivel()
    {
        dineroActual = dineroInicioNivel;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoDinero != null)
            textoDinero.text = $"${dineroActual}";
    }

    public void GuardarDinero()
    {
        SaveData currentSave = SaveManager.CargarTodo();
        SaveManager.GuardarTodo(
            currentSave.nivelActual,
            HistorialManager.Instance?.GetHistorialPedidos(),
            HistorialManager.Instance?.GetLibrosPrestados(),
            dineroActual,
            InventarioManager.Instance.ObtenerItems() 
        );
    }

    public void CargarDinero()
    {
        SaveData data = SaveManager.CargarTodo();
        dineroActual = data.dineroActual;
        dineroInicioNivel = dineroActual; 
        ActualizarUI();
    }
}