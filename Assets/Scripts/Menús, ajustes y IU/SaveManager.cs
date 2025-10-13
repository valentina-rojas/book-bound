using System.Collections.Generic;
using UnityEngine;

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
public class SlotPosicionGuardado
{
    public string slotPath;
    public string itemNombre;
}

[System.Serializable]
public class GeneroCantidad
{
    public string genero;
    public int cantidad;

    public GeneroCantidad() { }

    public GeneroCantidad(string genero, int cantidad)
    {
        this.genero = genero;
        this.cantidad = cantidad;
    }
}

[System.Serializable]
public class SaveData
{
    public int nivelActual;
    public int dinero;
    public List<LibroPrestado> librosPrestados;
    public List<LibroGuardado> librosEstantes;
    public List<SlotGuardado> slotsGuardados;
    public List<SlotPosicionGuardado> slotsCuadrosGuardados;
    public List<string> itemsInventario; 
    public List<GeneroCantidad> librosEsperadosPorGeneroList;
    public bool tiendaAbiertaPorPrimeraVez;
}

public static class SaveManager
{
    private const string SaveKey = "JuegoGuardado";

    #region Guardar
    public static void GuardarNivel(int nivel, List<LibroPrestado> libros)
    {
        List<LibroGuardado> librosEstantes = new List<LibroGuardado>();
        BookData[] todosLibros = GameObject.FindObjectsOfType<BookData>(true);
        foreach (BookData libro in todosLibros)
            librosEstantes.Add(libro.ToLibroGuardado());

        List<SlotGuardado> slotsGuardados = new List<SlotGuardado>();
        SlotLugar[] todosSlots = GameObject.FindObjectsOfType<SlotLugar>(true);
        foreach (SlotLugar slot in todosSlots)
            slotsGuardados.Add(slot.ToSlotGuardado());

        List<SlotPosicionGuardado> slotsPosicionGuardados = new List<SlotPosicionGuardado>();
        SlotPosicion[] todosSlotsPosicion = GameObject.FindObjectsOfType<SlotPosicion>(true);
        foreach (SlotPosicion slot in todosSlotsPosicion)
            slotsPosicionGuardados.Add(slot.ToSlotPosicionGuardado());

        List<string> itemsInventario = new List<string>();
        foreach (Item i in InventarioManager.Instance.ObtenerItems())
            itemsInventario.Add(i.nombre);

        List<GeneroCantidad> listaGeneros = new List<GeneroCantidad>();
        foreach (var kvp in ShelfManager.instance.librosEsperadosPorGenero)
            listaGeneros.Add(new GeneroCantidad(kvp.Key, kvp.Value));

        SaveData data = new SaveData()
        {
            nivelActual = nivel,
            dinero = EconomyManager.instance.ObtenerDinero(),
            librosPrestados = new List<LibroPrestado>(libros),
            librosEstantes = librosEstantes,
            slotsGuardados = slotsGuardados,
            slotsCuadrosGuardados = slotsPosicionGuardados,
            itemsInventario = itemsInventario,
            librosEsperadosPorGeneroList = listaGeneros,
            tiendaAbiertaPorPrimeraVez = TaskManager.instance != null ? 
                TaskManager.instance.SeAbrioTiendaAlMenosUnaVez() : false
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"Juego guardado: Nivel {nivel}, Libros: {librosEstantes.Count}, Slots: {slotsGuardados.Count}, SlotsCuadros: {slotsPosicionGuardados.Count}, ItemsInventario: {itemsInventario.Count}");
    }
    #endregion

    #region Cargar
    public static SaveData CargarNivel()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No hay partida guardada, comenzando nueva partida");
            int dineroInicial = EconomyManager.instance != null ? EconomyManager.instance.dineroInicial : 0;

            List<GeneroCantidad> listaGeneros = new List<GeneroCantidad>();
            if (ShelfManager.instance != null)
            {
                foreach (var kvp in ShelfManager.instance.librosEsperadosPorGenero)
                    listaGeneros.Add(new GeneroCantidad(kvp.Key, kvp.Value));
            }

            return new SaveData()
            {
                nivelActual = 1,
                dinero = dineroInicial,
                librosPrestados = new List<LibroPrestado>(),
                librosEstantes = new List<LibroGuardado>(),
                slotsGuardados = new List<SlotGuardado>(),
                slotsCuadrosGuardados = new List<SlotPosicionGuardado>(),
                itemsInventario = new List<string>(),
                librosEsperadosPorGeneroList = listaGeneros,
                tiendaAbiertaPorPrimeraVez = false
            };
        }

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Partida cargada: Nivel {data.nivelActual}, Libros: {data.librosEstantes.Count}, Slots: {data.slotsGuardados.Count}, SlotsCuadros: {data.slotsCuadrosGuardados.Count}, ItemsInventario: {data.itemsInventario.Count}");
        return data;
    }
    #endregion

    #region Restaurar
    public static void RestaurarDatos(SaveData data)
    {
        if (data == null) return;

        if (EconomyManager.instance != null)
            EconomyManager.instance.EstablecerDinero(data.dinero);

        RestaurarLibros(data);
        RestaurarSlots(data);
        RestaurarSlotsCuadros(data);
        RestaurarInventario(data);

        if (data.librosEsperadosPorGeneroList != null)
        {
            ShelfManager.instance.librosEsperadosPorGenero = new Dictionary<string, int>();
            foreach (var gc in data.librosEsperadosPorGeneroList)
                ShelfManager.instance.librosEsperadosPorGenero[gc.genero] = gc.cantidad;

            ShelfEstante[] estantes = GameObject.FindObjectsOfType<ShelfEstante>(true);
            foreach (var estante in estantes)
                estante.ActualizarCantidadEsperada();
        }
        SincronizarEstadoItems(data.itemsInventario);
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

        SlotPosicion[] todosSlots = GameObject.FindObjectsOfType<SlotPosicion>(true);

        foreach (SlotPosicionGuardado slotGuardado in data.slotsCuadrosGuardados)
        {
            SlotPosicion slot = System.Array.Find(todosSlots,
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

    public static void SincronizarEstadoItems(List<string> itemsInventario)
    {
        List<Item> todosLosItems = ShopManager.Instance != null ? ShopManager.Instance.ObtenerTodosLosItems() : null;
        if (todosLosItems == null) return;

        HashSet<string> itemsPresentes = new HashSet<string>(itemsInventario);

        SlotLugar[] slotsLugar = GameObject.FindObjectsOfType<SlotLugar>(true);
        foreach (var slot in slotsLugar)
        {
            if (slot.itemActual != null)
                itemsPresentes.Add(slot.itemActual.nombre);
        }

        SlotPosicion[] slotsCuadros = GameObject.FindObjectsOfType<SlotPosicion>(true);
        foreach (var slot in slotsCuadros)
        {
            if (slot.itemActual != null)
                itemsPresentes.Add(slot.itemActual.nombre);
        }

        foreach (var item in todosLosItems)
        {
            if (itemsPresentes.Contains(item.nombre))
            {
                item.comprado = true; 
            }
            else
            {
                item.comprado = false; 
            }
        }

        Debug.Log($"Sincronización de ítems completada. {itemsPresentes.Count} ítems encontrados en inventario o slots.");
    }
    #endregion

    #region Utilidades
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
    #endregion
}