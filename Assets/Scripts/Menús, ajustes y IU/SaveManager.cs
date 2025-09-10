using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
    public class SaveData
    {
        public int nivelActual;
        public List<LibroPrestado> librosPrestados;
    }

    public static class SaveManager
    {
        private const string SaveKey = "JuegoGuardado";

    public static void GuardarNivel(int nivel, List<LibroPrestado> libros)
    {
        SaveData data = new SaveData()
        {
            nivelActual = nivel,
            librosPrestados = new List<LibroPrestado>(libros)
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log($"Juego guardado: Nivel {nivel}, Libros: {libros.Count}");
    }

    public static SaveData CargarNivel()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return new SaveData()
            {
                nivelActual = 0,
                librosPrestados = new List<LibroPrestado>() 
            };
        }

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