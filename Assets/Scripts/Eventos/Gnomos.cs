using UnityEngine;

public class Gnomos : MonoBehaviour
{
    public static Gnomos instance;
    private bool eventoActivadoHoy = false;

    private bool desorganizarPendiente = false;

    private void Awake()
    {
        instance = this;
    }

    public void IntentarActivarEventoGnomos()
    {
        if (GameManager.instance.nivelActual >= 5 && !eventoActivadoHoy)
        {
            float probabilidad = 0.2f; 
            if (Random.value <= probabilidad)
            {
                Debug.Log("Los gnomos han entrado y pronto desorganizarán los estantes...");
                desorganizarPendiente = true;
            }
            else
            {
                Debug.Log("Los gnomos decidieron no hacer travesuras esta vez.");
            }
            eventoActivadoHoy = true;
        }
    }

    public void EjecutarDesorganizacionSiPendiente()
    {
        if (desorganizarPendiente && ShelfManager.instance != null)
        {
            ShelfManager.instance.DesorganizarLibros();
            desorganizarPendiente = false;
        }
    }

    public void ReiniciarEvento()
    {
        eventoActivadoHoy = false;
        desorganizarPendiente = false;
    }
}