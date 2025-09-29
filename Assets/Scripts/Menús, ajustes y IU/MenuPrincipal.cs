using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Panel de Consentimiento")]
    [SerializeField] private GameObject panelConsentimiento;

    private string escenaDestino;
    private System.Action accionDespuesDelConsentimiento;

    public void NuevaPartida()
    {
        SaveManager.BorrarGuardado();

        accionDespuesDelConsentimiento = () =>
        {
            SceneManager.LoadScene("Cinematica");
            foreach (Item item in InventarioManager.Instance.todosLosItems)
                item.comprado = false;
        };

        escenaDestino = "Cinematica";
        panelConsentimiento.SetActive(true); 
    }

    public void CargarPartida()
    {
        SaveData data = SaveManager.CargarNivel();

        accionDespuesDelConsentimiento = null;

        if (data == null || data.nivelActual <= 1)
        {
            accionDespuesDelConsentimiento = () =>
            {
                SceneManager.LoadScene("Cinematica");
                foreach (Item item in InventarioManager.Instance.todosLosItems)
                    item.comprado = false;
            };

            escenaDestino = "Cinematica";
        }
        else
        {
            accionDespuesDelConsentimiento = () =>
            {
                SceneManager.LoadScene("Gameplay");
            };

            escenaDestino = "Gameplay";
        }

        panelConsentimiento.SetActive(true); 
    }

    public void AceptarConsentimiento()
    {
        if (Services.Instance == null)
        {
            GameObject go = new GameObject("Services");
            go.AddComponent<Services>();
        }

        Services.Instance.StartDataCollection();
        EjecutarAccionDespuesDelConsentimiento();
    }

    public void RechazarConsentimiento()
    {
        if (Services.Instance == null)
        {
            GameObject go = new GameObject("Services");
            go.AddComponent<Services>();
        }

        Services.Instance.StopDataCollection();
        EjecutarAccionDespuesDelConsentimiento();
    }

    private void EjecutarAccionDespuesDelConsentimiento()
    {
        panelConsentimiento.SetActive(false);
        accionDespuesDelConsentimiento?.Invoke();
    }
}