using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    }

    public void CerrarTienda()
    {
        panelTienda.SetActive(false);
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

            ActualizarUI();
        }
        else
        {
            Debug.Log("No tienes suficiente dinero para comprar " + item.nombre);
        }
    }
    #endregion
}