using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;  

public class HechizoManager : MonoBehaviour
{
    public static HechizoManager instance;

    public Button botonEntregarHechizo;

    [Header("Botones de runas")]
    public Button[] botonesRunas;

    [Header("Nombres de runas")]
    public string[] nombresRunas = { "Fuego", "Agua", "Tierra", "Aire", "Luz", "Sombra" };

    [Header("Texto para mensajes en pantalla")]
    public TMP_Text mensajeEnPantalla;  

    private List<int> secuenciaSeleccionada = new List<int>();

    private Vector3 escalaOriginal = Vector3.one;
    private Vector3 escalaSeleccionada = Vector3.one * 1.2f;

    private Dictionary<string, List<int>> hechizos = new Dictionary<string, List<int>>();

    private string hechizoFormado = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        hechizos.Add("Sellado", new List<int> { 2, 5, 0 });       // Tierra, Sombra, Fuego
        hechizos.Add("Protección", new List<int> { 4, 2, 1 });    // Luz, Tierra, Agua
        hechizos.Add("Traducción", new List<int> { 0, 3, 4 });    // Fuego, Aire, Luz
        hechizos.Add("Restauración", new List<int> { 1, 2, 0 });  // Agua, Tierra, Fuego
        hechizos.Add("Comunicación", new List<int> { 3, 4, 1 });  // Aire, Luz, Agua

        botonEntregarHechizo.interactable = false;

        if (mensajeEnPantalla != null)
            mensajeEnPantalla.gameObject.SetActive(false);

        for (int i = 0; i < botonesRunas.Length; i++)
        {
            int index = i;
            botonesRunas[i].onClick.AddListener(() => OnRunasClick(index));
        }
    }

    private void OnRunasClick(int indiceRuna)
    {
        if (secuenciaSeleccionada.Count >= 3)
            return;

        botonesRunas[indiceRuna].transform.localScale = escalaSeleccionada;

        secuenciaSeleccionada.Add(indiceRuna);

        if (secuenciaSeleccionada.Count == 3)
        {
            VerificarHechizo();
        }
    }

    private void VerificarHechizo()
    {
        foreach (var kvp in hechizos)
        {
            if (SonIguales(kvp.Value, secuenciaSeleccionada))
            {
                hechizoFormado = kvp.Key;
                Debug.Log($"¡Hechizo formado: {hechizoFormado}!");
                botonEntregarHechizo.interactable = true;
                return;
            }
        }

        hechizoFormado = null;
        if (mensajeEnPantalla != null)
            StartCoroutine(MostrarMensajeTemporal("No pasó nada...", 2f));

        ResetearSecuencia();
    }

    private bool SonIguales(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }

    private void ResetearSecuencia()
    {
        foreach (int i in secuenciaSeleccionada)
        {
            botonesRunas[i].transform.localScale = escalaOriginal;
        }
        secuenciaSeleccionada.Clear();
        botonEntregarHechizo.interactable = false;
    }

    private CharacterAttributes.Hechizo ConvertirStringAEnum(string nombreHechizo)
    {
        switch (nombreHechizo)
        {
            case "Sellado":
                return CharacterAttributes.Hechizo.Sellado;
            case "Protección":
                return CharacterAttributes.Hechizo.Proteccion;
            case "Traducción":
                return CharacterAttributes.Hechizo.Traduccion;
            case "Restauración":
                return CharacterAttributes.Hechizo.Restauracion;
            case "Comunicación":
                return CharacterAttributes.Hechizo.Comunicacion;
            default:
                return CharacterAttributes.Hechizo.Ninguno;
        }
    }

    public void EntregarLibroHechizado()
    {
        if (!botonEntregarHechizo.interactable || string.IsNullOrEmpty(hechizoFormado))
        {
            Debug.LogWarning("No se puede entregar hechizo, secuencia inválida.");
            return;
        }

        Debug.Log($"Libro hechizado entregado correctamente con hechizo: {hechizoFormado}");

        CharacterAttributes.Hechizo hechizoEnum = ConvertirStringAEnum(hechizoFormado);

        GameManager.instance.CompletarHechizo(hechizoEnum);

        ResetearSecuencia();
        CameraManager.instance.DesctivarPanelHechizo();
       

        hechizoFormado = null;
    }

    private IEnumerator MostrarMensajeTemporal(string mensaje, float duracion)
    {
        mensajeEnPantalla.text = mensaje;
        mensajeEnPantalla.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracion);

        mensajeEnPantalla.gameObject.SetActive(false);
    }
}

