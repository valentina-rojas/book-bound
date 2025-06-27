using UnityEngine;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    public int totalPlants = 0;
    private int fullyWateredPlants = 0;
    private List<PlantWithRegadera> plantas = new List<PlantWithRegadera>();

    public int expectedPlantCount = 4;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterPlant(PlantWithRegadera planta)
    {
        plantas.Add(planta);
        totalPlants = plantas.Count;

        if (plantas.Count == expectedPlantCount)
        {
            Debug.Log("Todas las plantas registradas.");
        }
    }

    public void NotifyPlantFullyWatered()
    {
        fullyWateredPlants++;

        if (fullyWateredPlants >= totalPlants)
        {
            Debug.Log("¡Todas las plantas fueron regadas!");
            TaskManager.instance.CompletarTareaPorID(4);
        }
    }

    public void ReiniciarEstado()
    {
        fullyWateredPlants = 0;

        foreach (var planta in plantas)
        {
            planta.ReiniciarPlanta();
        }
    }
}