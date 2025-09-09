using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int nivelActual;
    public List<string> historialPedidos;
    public List<string> librosPrestados;
    public int dineroActual;
    public List<string> itemsInventario; 
}

public static class SaveManager
{
    private const string SaveKey = "JuegoGuardado";

    public static void GuardarTodo(int nivel, List<string> historial, List<string> libros, int dinero, List<Item> items)
    {
        SaveData data = new SaveData()
        {
            nivelActual = nivel,
            historialPedidos = historial ?? new List<string>(),
            librosPrestados = libros ?? new List<string>(),
            dineroActual = dinero,
            itemsInventario = new List<string>()
        };

        if (items != null)
        {
            foreach (var item in items)
                data.itemsInventario.Add(item.nombre);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log($"Juego guardado: Nivel {nivel}, {historial.Count} pedidos, {libros.Count} libros, Dinero: {dinero}, Items: {data.itemsInventario.Count}");
    }

    public static SaveData CargarTodo()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return new SaveData()
            {
                nivelActual = 1,
                historialPedidos = new List<string>(),
                librosPrestados = new List<string>(),
                dineroActual = 0,
                itemsInventario = new List<string>()
            };

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static void BorrarGuardado()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Guardado eliminado");
    }
}