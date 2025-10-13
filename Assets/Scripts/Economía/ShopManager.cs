using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Services.Analytics;
using static EventManager;

public class ShopManager : MonoBehaviour
{
    #region Singleton
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelTienda.SetActive(false);
        InicializarBotonesCategorias();
        timerTienda = new Stopwatch();

        comprasPorItem = new Dictionary<string, int>();
        contadorTotalCompras = 0; 
    }
    #endregion

    #region UI References
    [Header("UI Tienda")]
    [SerializeField] private GameObject panelTienda;
    [SerializeField] private Transform contenedorItems;
    [SerializeField] private GameObject prefabSlotTienda;
    [SerializeField] private List<Button> botonesCategorias;
    #endregion

    #region Items disponibles
    [Header("Items disponibles en la tienda")]
    [SerializeField] private List<Item> itemsDisponibles;
    private CategoriaItem categoriaActual = CategoriaItem.Pisos;
    #endregion

    #region Analytics
    private Stopwatch timerTienda;
    private bool openedOnce = false;
    private bool intentoFallidoSesion = false;
    private Dictionary<string, int> comprasPorItem;
    private int contadorTotalCompras; 
    #endregion

    #region Inicialización
    private void InicializarBotonesCategorias()
    {
        for (int i = 0; i < botonesCategorias.Count; i++)
        {
            int index = i;
            botonesCategorias[i].onClick.AddListener(() => CambiarCategoria((CategoriaItem)index));
        }
    }
    #endregion

    #region Abrir/Cerrar Tienda
    public void AbrirTienda()
    {
        panelTienda.SetActive(true);
        CambiarCategoria(categoriaActual);

        if (!openedOnce)
            openedOnce = true;

        timerTienda.Reset();
        timerTienda.Start();

        intentoFallidoSesion = false;
        comprasPorItem.Clear(); 
    }

    public void CerrarTienda()
    {
        panelTienda.SetActive(false);

        timerTienda.Stop();
        int segundos = (int)(timerTienda.ElapsedMilliseconds / 1000f);

        RegistrarEventoTienda(segundos);
    }
    #endregion

    #region Manejo de Categorías
    public void CambiarCategoria(CategoriaItem nuevaCategoria)
    {
        categoriaActual = nuevaCategoria;
        ActualizarUI();
    }
    #endregion

    #region Actualizar UI
    private void ActualizarUI()
    {
        foreach (Transform child in contenedorItems)
            Destroy(child.gameObject);

        foreach (var item in itemsDisponibles)
        {
            if (item.categoria != categoriaActual) continue;


            if (item.categoria == CategoriaItem.Herramientas || !item.comprado)
            {
                GameObject nuevoSlot = Instantiate(prefabSlotTienda, contenedorItems);
                UITiendaSlot slot = nuevoSlot.GetComponent<UITiendaSlot>();
                slot.Configurar(item, ComprarItem);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contenedorItems.GetComponent<RectTransform>());
    }
    #endregion

    #region Compras
    private void ComprarItem(Item item)
    {
        if (EconomyManager.instance.ObtenerDinero() >= item.precio)
        {
            EconomyManager.instance.RestarDinero(item.precio);
            InventarioManager.Instance.AgregarItemSinAbrir(item);

            item.comprado = true;
            contadorTotalCompras++;

            if (comprasPorItem.ContainsKey(item.nombre))
                comprasPorItem[item.nombre]++;
            else
                comprasPorItem[item.nombre] = 1;

            ActualizarUI();
        }
        else
        {
            UnityEngine.Debug.Log("No tienes suficiente dinero para comprar " + item.nombre);
            intentoFallidoSesion = true;
        }
    }
    #endregion

    #region Analytics Métodos
    private void RegistrarEventoTienda(int segundos)
    {
        TiendaEvent tiendaEvent = new TiendaEvent();
        tiendaEvent.opened = openedOnce;
        tiendaEvent.timeInShop = segundos;
        tiendaEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(tiendaEvent);
#else
        UnityEngine.Debug.Log($"[ANALYTICS] TiendaEvent: opened={openedOnce}, timeInShop={segundos}, level={GameManager.instance.nivelActual}");
#endif

        foreach (var kvp in comprasPorItem)
        {
            TiendaCompraEvent compraEvent = new TiendaCompraEvent();
            compraEvent.itemName = kvp.Key;
            compraEvent.cant = kvp.Value;
            compraEvent.failed = intentoFallidoSesion;
            compraEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent(compraEvent);
#else
            UnityEngine.Debug.Log($"[ANALYTICS] TiendaCompraEvent: itemName={kvp.Key}, cant={kvp.Value}, failed={intentoFallidoSesion}, level={GameManager.instance.nivelActual}");
#endif
        }

        if (contadorTotalCompras > 0)
        {
            TiendaCompraEvent totalEvent = new TiendaCompraEvent();
            totalEvent.itemName = "TotalItemsComprados";
            totalEvent.cant = contadorTotalCompras;
            totalEvent.failed = intentoFallidoSesion;
            totalEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent(totalEvent);
#else
            UnityEngine.Debug.Log($"[ANALYTICS] TiendaCompraEvent: itemName=TotalItemsComprados, cant={contadorTotalCompras}, failed={intentoFallidoSesion}, level={GameManager.instance.nivelActual}");
#endif
        }
    }
    #endregion

    #region Métodos públicos adicionales
    public List<Item> ObtenerItemsDisponibles()
    {
        return itemsDisponibles;
    }

    public List<Item> ObtenerTodosLosItems()
    {
        return itemsDisponibles;
    }
    #endregion
}