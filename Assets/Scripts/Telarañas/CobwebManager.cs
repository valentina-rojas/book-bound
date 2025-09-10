using System.Collections.Generic;
using UnityEngine;

public class CobwebManager : MonoBehaviour
{
    public static CobwebManager instance;

    private GameManager gameManager;
    private List<CobwebCleaning> todasLasTelarañas = new List<CobwebCleaning>();
    private List<CobwebCleaning> telarañasActivas = new List<CobwebCleaning>();
    private bool gameManagerListo = false;

    private const int maxPorSala = 3; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        todasLasTelarañas.Clear();
        todasLasTelarañas.AddRange(FindObjectsOfType<CobwebCleaning>(true));

        if (gameManager == null)
            Debug.LogError("GameManager no encontrado en la escena.");
        else
        {
            gameManagerListo = true;
            ActivarTelarañasPorNivel(gameManager.nivelActual);
        }
    }

    public void RegistrarTelaraña(CobwebCleaning telaraña)
    {
        if (!todasLasTelarañas.Contains(telaraña))
            todasLasTelarañas.Add(telaraña);
    }

    public void EliminarTelaraña(CobwebCleaning telaraña)
    {
        telarañasActivas.Remove(telaraña);
        if (telarañasActivas.Count == 0)
            TaskManager.instance.CompletarTareaPorID(0);
    }

    public void ReiniciarTelarañas()
    {
        telarañasActivas.Clear();

        foreach (CobwebCleaning t in todasLasTelarañas)
        {
            if (t != null)   
                t.ReiniciarTelaraña();
        }

        if (gameManagerListo && gameManager != null)
            ActivarTelarañasPorNivel(gameManager.nivelActual);
    }

    public void ActivarTelarañasPorNivel(int nivelActual)
    {
        foreach (var t in todasLasTelarañas)
            t.gameObject.SetActive(false);

        telarañasActivas.Clear();

        Dictionary<string, List<CobwebCleaning>> telarañasPorSala = new Dictionary<string, List<CobwebCleaning>>();

        foreach (var t in todasLasTelarañas)
        {
            if (t.esTelarañaTutorial)
            {
                if (nivelActual == 1)
                {
                    t.gameObject.SetActive(true);
                    t.puedeInteractuar = false;
                    t.interaccionFueHabilitada = false;

                    if (!telarañasActivas.Contains(t))
                        telarañasActivas.Add(t);
                }
                continue; 
            }

            if (nivelActual == 1)
                continue;

            if (t.nivelMinimo <= nivelActual)
            {
                if (!telarañasPorSala.ContainsKey(t.sala))
                    telarañasPorSala[t.sala] = new List<CobwebCleaning>();

                telarañasPorSala[t.sala].Add(t);
            }
        }

        if (nivelActual > 1)
        {
            foreach (var sala in telarañasPorSala.Keys)
            {
                List<CobwebCleaning> lista = telarañasPorSala[sala];

                for (int i = 0; i < lista.Count; i++)
                {
                    CobwebCleaning temp = lista[i];
                    int randomIndex = Random.Range(i, lista.Count);
                    lista[i] = lista[randomIndex];
                    lista[randomIndex] = temp;
                }

                int cantidadActivar = Random.Range(1, Mathf.Min(maxPorSala, lista.Count) + 1);

                for (int i = 0; i < cantidadActivar; i++)
                {
                    var t = lista[i];
                    t.gameObject.SetActive(true);
                    if (!telarañasActivas.Contains(t))
                        telarañasActivas.Add(t);
                }
            }
        }

        Debug.Log($"Nivel {nivelActual}: Activadas {telarañasActivas.Count} telarañas (Tutorial incluida: {nivelActual == 1})");
    }

}