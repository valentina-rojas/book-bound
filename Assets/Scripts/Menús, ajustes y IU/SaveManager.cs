using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int nivelActual;
}

public static class SaveManager
{
    private const string SaveKey = "JuegoGuardado";

    public static void GuardarNivel(int nivel)
    {
        SaveData data = new SaveData()
        {
            nivelActual = nivel
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log($"Juego guardado: Nivel {nivel}");
    }

    public static SaveData CargarNivel()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return new SaveData()
            {
                nivelActual = 0
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