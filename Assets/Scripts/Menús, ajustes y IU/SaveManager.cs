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
public class SlotGuardado
{
    public string slotPath;
    public string itemNombre;
}

[System.Serializable]
public class SlotCuadroGuardado
{
    public string slotPath;
    public string itemNombre;
}

[System.Serializable]
public class SaveData
{
    public int nivelActual;
    public int dinero;
    public List<LibroPrestado> librosPrestados;
    public List<LibroGuardado> librosEstantes;
    public List<SlotGuardado> slotsGuardados;
    public List<SlotCuadroGuardado> slotsCuadrosGuardados;
    public List<string> itemsInventario; // Items que están en el inventario
}

public static class SaveManager
{
    private const string SaveKey = "JuegoGuardado";

    // ------------------ GUARDAR ------------------
    public static void GuardarNivel(int nivel, List<LibroPrestado> libros)
    {
        // Guardar libros en estantes
        List<LibroGuardado> librosEstantes = new List<LibroGuardado>();
        BookData[] todosLibros = GameObject.FindObjectsOfType<BookData>(true);
        foreach (BookData libro in todosLibros)
            librosEstantes.Add(libro.ToLibroGuardado());

        // Guardar slots normales
        List<SlotGuardado> slotsGuardados = new List<SlotGuardado>();
        SlotLugar[] todosSlots = GameObject.FindObjectsOfType<SlotLugar>(true);
        foreach (SlotLugar slot in todosSlots)
            slotsGuardados.Add(slot.ToSlotGuardado());

        // Guardar slots de cuadros
        List<SlotCuadroGuardado> slotsCuadrosGuardados = new List<SlotCuadroGuardado>();
        SlotCuadro[] todosSlotsCuadros = GameObject.FindObjectsOfType<SlotCuadro>(true);
        foreach (SlotCuadro slot in todosSlotsCuadros)
            slotsCuadrosGuardados.Add(slot.ToSlotCuadroGuardado());

        // Guardar items en inventario
        List<string> itemsInventario = new List<string>();
        foreach (Item i in InventarioManager.Instance.ObtenerItems())
            itemsInventario.Add(i.nombre);

        // Crear objeto SaveData
        SaveData data = new SaveData()
        {
            nivelActual = nivel,
            dinero = EconomyManager.instance.ObtenerDinero(),
            librosPrestados = new List<LibroPrestado>(libros),
            librosEstantes = librosEstantes,
            slotsGuardados = slotsGuardados,
            slotsCuadrosGuardados = slotsCuadrosGuardados,
            itemsInventario = itemsInventario
        };

        // Guardar en PlayerPrefs
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Juego guardado: Nivel {nivel}, Libros: {librosEstantes.Count}, Slots: {slotsGuardados.Count}, SlotsCuadros: {slotsCuadrosGuardados.Count}, ItemsInventario: {itemsInventario.Count}");
    }

    // ------------------ CARGAR ------------------
    public static SaveData CargarNivel()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No hay partida guardada, comenzando nueva partida");
            int dineroInicial = EconomyManager.instance != null ? EconomyManager.instance.dineroInicial : 0;

            return new SaveData()
            {
                nivelActual = 1,
                dinero = dineroInicial, // ← usa el valor inicial configurado
                librosPrestados = new List<LibroPrestado>(),
                librosEstantes = new List<LibroGuardado>(),
                slotsGuardados = new List<SlotGuardado>(),
                slotsCuadrosGuardados = new List<SlotCuadroGuardado>(),
                itemsInventario = new List<string>()
            };
        }

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Partida cargada: Nivel {data.nivelActual}, Libros: {data.librosEstantes.Count}, Slots: {data.slotsGuardados.Count}, SlotsCuadros: {data.slotsCuadrosGuardados.Count}, ItemsInventario: {data.itemsInventario.Count}");
        return data;
    }

    // ------------------ RESTAURAR ------------------
    public static void RestaurarDatos(SaveData data)
    {
        if (data == null) return;

        if (EconomyManager.instance != null)
            EconomyManager.instance.EstablecerDinero(data.dinero);

        RestaurarLibros(data);
        RestaurarSlots(data);
        RestaurarSlotsCuadros(data);
        RestaurarInventario(data);
    }

    public static void RestaurarLibros(SaveData data)
    {
        if (data == null || data.librosEstantes == null || data.librosEstantes.Count == 0)
            return;

        BookData[] todosLibros = GameObject.FindObjectsOfType<BookData>(true);
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
                        libro.transform.SetParent(slotPadre.transform);
                }

                libro.transform.localPosition = libroGuardado.posicion;
                libro.gameObject.SetActive(libroGuardado.estaHabilitado);
            }
        }
    }

    public static void RestaurarSlots(SaveData data)
    {
        if (data == null || data.slotsGuardados == null) return;

        SlotLugar[] todosSlots = GameObject.FindObjectsOfType<SlotLugar>(true);

        foreach (SlotGuardado slotGuardado in data.slotsGuardados)
        {
            SlotLugar slot = System.Array.Find(todosSlots,
                s => ObtenerRutaCompleta(s.transform) == slotGuardado.slotPath
            );

            if (slot != null)
            {
                if (!string.IsNullOrEmpty(slotGuardado.itemNombre))
                {
                    Item item = InventarioManager.Instance.ObtenerItemPorNombre(slotGuardado.itemNombre);
                    slot.itemActual = item;
                    if (slot.render != null && item != null && item.icono != null)
                        slot.render.sprite = item.icono;
                }
                else
                {
                    slot.itemActual = null;
                    if (slot.render != null) slot.render.sprite = null;
                }
            }
        }
    }

    public static void RestaurarSlotsCuadros(SaveData data)
    {
        if (data == null || data.slotsCuadrosGuardados == null) return;

        SlotCuadro[] todosSlots = GameObject.FindObjectsOfType<SlotCuadro>(true);

        foreach (SlotCuadroGuardado slotGuardado in data.slotsCuadrosGuardados)
        {
            SlotCuadro slot = System.Array.Find(todosSlots,
                s => ObtenerRutaCompleta(s.transform) == slotGuardado.slotPath
            );

            if (slot != null)
            {
                if (!string.IsNullOrEmpty(slotGuardado.itemNombre))
                {
                    Item item = InventarioManager.Instance.ObtenerItemPorNombre(slotGuardado.itemNombre);
                    slot.TryColocarItem(item, out _);
                }
                else
                {
                    slot.QuitarItem();
                }
            }
        }
    }

    public static void RestaurarInventario(SaveData data)
    {
        if (data == null || data.itemsInventario == null) return;

        InventarioManager inventario = InventarioManager.Instance;
        foreach (string itemNombre in data.itemsInventario)
        {
            Item item = inventario.ObtenerItemPorNombre(itemNombre);
            if (item != null)
                inventario.AgregarItemSinAbrir(item);
        }
    }

    // ------------------ UTILIDADES ------------------
    public static string ObtenerRutaCompleta(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    public static void BorrarGuardado()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}
