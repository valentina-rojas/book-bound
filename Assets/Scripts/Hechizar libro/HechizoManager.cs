using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;  
using Unity.Services.Analytics;
using static EventManager; 

public class HechizoManager : MonoBehaviour
{
    public static HechizoManager instance;

    public Button botonEntregarHechizo;

    [Header("Botones de runas")]
    public Button[] botonesRunas;

    [Header("Nombres de runas")]
    public string[] nombresRunas = { "Fuego", "Agua", "Tierra", "Aire", "Luz", "Sombra" };

    [Header("Partículas de runas")]
    public ParticleSystem[] particulasRunas; 

    [Header("Texto para mensajes en pantalla")]
    public TMP_Text mensajeEnPantalla;  

    private List<int> secuenciaSeleccionada = new List<int>();
    private Dictionary<int, ParticleSystem> particulasActivas = new Dictionary<int, ParticleSystem>();

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
        AudioManager.instance.encanto.Play();

        GenerarParticulasRuna(indiceRuna);

        if (secuenciaSeleccionada.Count == 3)
        {
            VerificarHechizo();
        }
    }

    private void GenerarParticulasRuna(int indiceRuna)
    {
        if (particulasRunas == null || particulasRunas.Length <= indiceRuna || particulasRunas[indiceRuna] == null)
            return;

        if (particulasActivas.ContainsKey(indiceRuna)) return;

        Transform runaTransform = botonesRunas[indiceRuna].transform;
        Transform contenedor = runaTransform.parent; 
        ParticleSystem particulas = Instantiate(particulasRunas[indiceRuna], contenedor);

        particulas.transform.position = runaTransform.position;
        particulas.Play();
        particulasActivas[indiceRuna] = particulas;
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

            if (particulasActivas.ContainsKey(i))
            {
                ParticleSystem p = particulasActivas[i];
                if (p != null)
                {
                    p.Stop();
                    Destroy(p.gameObject, 0.5f);
                }
            }
        }

        particulasActivas.Clear();
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

        bool hechizoCorrecto = (hechizoEnum == GameManager.instance.personajeActual.hechizoSolicitado);

        GameManager.instance.CompletarHechizo(hechizoEnum);

        RegistrarEventoHechizo(hechizoCorrecto);

        ResetearSecuencia();
        CameraManager.instance.DesctivarPanelHechizo();
        hechizoFormado = null;
    }

    private void RegistrarEventoHechizo(bool hechizoCorrecto)
    {
        HechizoEvent hechizoEvent = new HechizoEvent();
        hechizoEvent.spell = hechizoCorrecto;
        hechizoEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
        Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(hechizoEvent);
#else
        Debug.Log($"[ANALYTICS] HechizoEvent: spell={hechizoCorrecto}, level={GameManager.instance.nivelActual}");
#endif
    }

    private IEnumerator MostrarMensajeTemporal(string mensaje, float duracion)
    {
        mensajeEnPantalla.text = mensaje;
        mensajeEnPantalla.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracion);

        mensajeEnPantalla.gameObject.SetActive(false);
    }
}
