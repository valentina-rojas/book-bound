using UnityEngine;
using System.Diagnostics; 
using Unity.Services.Analytics;
using static EventManager; 

public class TraduccionManager : MonoBehaviour
{
    public static TraduccionManager instance;
    private CharacterSpawn characterSpawn;
    private Stopwatch timerTraduccion;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        characterSpawn = FindFirstObjectByType<CharacterSpawn>();
        if (characterSpawn == null)
        {
            UnityEngine.Debug.LogError("CharacterSpawn no encontrado por TraduccionManager.");
        }

        timerTraduccion = new Stopwatch();
    }

    public void IniciarTraduccion(CharacterAttributes personaje)
    {
        if (personaje == null) return;

        string clave = personaje.claveMensajeCriptograma;
        CriptogramaManager.instance.GenerarCriptogramaPorClave(clave);
        CameraManager.instance.ActivarPanelTraduccion();
        timerTraduccion.Reset();
        timerTraduccion.Start();
    }

    public void EntregarTraduccion()
    {
        timerTraduccion.Stop();
        int segundos = (int)(timerTraduccion.ElapsedMilliseconds / 1000f);

        GameManager.instance.CompletarTraduccion();
        CameraManager.instance.DesctivarPanelTraduccion();
        AudioManager.instance.sonidoLibroCorrecto.Play();

        RegistrarEventoTraduccion(segundos);

        if (characterSpawn != null)
            characterSpawn.EndInteraction();
    }

    private void RegistrarEventoTraduccion(int segundos)
    {
        TraduccionEvent traduccionEvent = new TraduccionEvent();
        traduccionEvent.timeTranslation = segundos;
        traduccionEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
        Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(traduccionEvent);
#else
        UnityEngine.Debug.Log($"[ANALYTICS] TraduccionEvent: timeTranslation={segundos}, level={GameManager.instance.nivelActual}");
#endif
    }
}
