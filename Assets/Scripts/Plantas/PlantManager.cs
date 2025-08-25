using UnityEngine;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    private List<PlantWithRegadera> todasLasPlantas = new List<PlantWithRegadera>();
    private List<PlantWithRegadera> plantasActivas = new List<PlantWithRegadera>();

    private int fullyWateredPlants = 0;
    private GameManager gameManager;

    private const int maxPorDia = 5; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
            ActivarPlantasPorNivel(gameManager.nivelActual);
    }

    public void RegisterPlant(PlantWithRegadera planta)
    {
        if (!todasLasPlantas.Contains(planta))
            todasLasPlantas.Add(planta);
    }

    public void NotifyPlantFullyWatered()
    {
        fullyWateredPlants++;

        if (fullyWateredPlants >= plantasActivas.Count && plantasActivas.Count > 0)
        {
            TaskManager.instance.CompletarTareaPorID(4);
        }
    }

    public void ReiniciarEstado()
    {
        fullyWateredPlants = 0;
        plantasActivas.Clear();

        foreach (var planta in todasLasPlantas)
            planta.ReiniciarPlanta();

        if (gameManager != null)
            ActivarPlantasPorNivel(gameManager.nivelActual);
    }

    public void ActivarPlantasPorNivel(int nivelActual)
    {
        plantasActivas.Clear();

        foreach (var p in todasLasPlantas)
            p.ReiniciarPlanta();

        List<PlantWithRegadera> candidatas = todasLasPlantas.FindAll(p => p.nivelMinimo <= nivelActual);

        if (candidatas.Count == 0) return;

        for (int i = 0; i < candidatas.Count; i++)
        {
            PlantWithRegadera temp = candidatas[i];
            int randomIndex = Random.Range(i, candidatas.Count);
            candidatas[i] = candidatas[randomIndex];
            candidatas[randomIndex] = temp;
        }

        int cantidadActivar = Random.Range(1, Mathf.Min(maxPorDia, candidatas.Count) + 1);

        for (int i = 0; i < cantidadActivar; i++)
        {
            var p = candidatas[i];
            p.ActivarHoy();
            plantasActivas.Add(p);
        }

        Debug.Log($"Nivel {nivelActual}: {plantasActivas.Count} plantas marchitas hoy.");
    }
}