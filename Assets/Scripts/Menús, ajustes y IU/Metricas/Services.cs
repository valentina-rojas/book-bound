using System;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;

public class Services : MonoBehaviour
{
    public static Services Instance { get; private set; }

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void StartDataCollection()
    {
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Recolección de datos activada.");
    }

    public void StopDataCollection()
    {
        AnalyticsService.Instance.StopDataCollection();
        Debug.Log("Recolección de datos desactivada.");
    }
}