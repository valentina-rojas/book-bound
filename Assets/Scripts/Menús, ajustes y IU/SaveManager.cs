using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LibroGuardado
{
    public int libroID;
    public string tipoLibro;
    public string titulo;
    public string descripcion;
    public bool estaHabilitado;
    public Vector3 posicion;
    public string parentPath;
}

[System.Serializable]
public class SaveData
{
    public int nivelActual;
    public int dinero;
    public List<LibroPrestado> librosPrestados;
    public List<LibroGuardado> librosEstantes;
}

public static class SaveManager
{
    private const string SaveKey = "JuegoGuardado";

    public static void GuardarNivel(int nivel, List<LibroPrestado> libros)
    {
        List<LibroGuardado> librosEstantes = new List<LibroGuardado>();
        BookData[] todosLibros = GameObject.FindObjectsOfType<BookData>(true); 
        
        foreach (BookData libro in todosLibros)
        {
            librosEstantes.Add(libro.ToLibroGuardado());
        }

        SaveData data = new SaveData()
        {
            nivelActual = nivel,
            dinero = EconomyManager.instance.ObtenerDinero(),
            librosPrestados = new List<LibroPrestado>(libros),
            librosEstantes = librosEstantes
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log($"Juego guardado: Nivel {nivel}, Libros prestados: {libros.Count}, Libros en estantes: {librosEstantes.Count}");
    }

    public static SaveData CargarNivel()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No hay partida guardada, comenzando nueva partida");
            return new SaveData()
            {
                nivelActual = 1,
                dinero = 0,
                librosPrestados = new List<LibroPrestado>(),
                librosEstantes = new List<LibroGuardado>()
            };
        }

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Partida cargada: Nivel {data.nivelActual}, Libros en estantes: {data.librosEstantes.Count}");
        return data;
    }
    public static void RestaurarDatos(SaveData data)
    {
        if (data == null) return;

        if (EconomyManager.instance != null)
            EconomyManager.instance.EstablecerDinero(data.dinero);
            
        RestaurarLibros(data);
    }
    public static void RestaurarLibros(SaveData data)
    {
        if (data == null || data.librosEstantes == null || data.librosEstantes.Count == 0)
        {
            Debug.Log("No hay datos de libros para restaurar");
            return;
        }

        BookData[] todosLibros = GameObject.FindObjectsOfType<BookData>(true);
        Debug.Log($"Encontré {todosLibros.Length} libros en la escena para restaurar");
        ShelfSlots[] todosLosSlots = GameObject.FindObjectsOfType<ShelfSlots>(true);

        foreach (LibroGuardado libroGuardado in data.librosEstantes)
        {
            BookData libro = System.Array.Find(todosLibros, l => l.libroID == libroGuardado.libroID);

            if (libro != null)
            {
                if (!string.IsNullOrEmpty(libroGuardado.parentPath))
                {
                    ShelfSlots slotPadre = System.Array.Find(todosLosSlots, s => s.gameObject.name == libroGuardado.parentPath);
                    if (slotPadre != null)
                    {
                        libro.transform.SetParent(slotPadre.transform);
                    }
                }

                libro.transform.localPosition = libroGuardado.posicion;
                libro.gameObject.SetActive(libroGuardado.estaHabilitado);

                Debug.Log($"Libro {libroGuardado.libroID} restaurado en slot: {libroGuardado.parentPath}");
            }
            else
            {
                Debug.LogWarning($"No se encontró el libro con ID {libroGuardado.libroID} en la escena");
            }
        }

        if (ShelfManager.instance != null)
        {
            ShelfManager.instance.Invoke("ForzarVerificacionTodosEstantes", 0.3f);
            ShelfManager.instance.Invoke("RevisarOrganizacion", 0.5f);
        }
    }

    public static void BorrarGuardado()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Guardado eliminado");
    }
}