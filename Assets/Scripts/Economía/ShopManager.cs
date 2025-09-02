using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Tienda")]
    [SerializeField] private GameObject panelTienda;
    [SerializeField] private Transform contenedorItems; 
    [SerializeField] private GameObject prefabSlotTienda;
    [SerializeField] private List<Button> botonesCategorias; 

    [Header("Items disponibles en la tienda")]
    [SerializeField] private List<Item> itemsDisponibles;

    private CategoriaItem categoriaActual = CategoriaItem.Pisos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelTienda.SetActive(false);

        for (int i = 0; i < botonesCategorias.Count; i++)
        {
            int index = i;
            botonesCategorias[i].onClick.AddListener(() => CambiarCategoria((CategoriaItem)index));
        }
    }

    public void AbrirTienda()
    {
        panelTienda.SetActive(true);
        CambiarCategoria(categoriaActual); 
    }

    public void CerrarTienda()
    {
        panelTienda.SetActive(false);
    }

    public void CambiarCategoria(CategoriaItem nuevaCategoria)
    {
        categoriaActual = nuevaCategoria;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        foreach (Transform child in contenedorItems)
        {
            Destroy(child.gameObject);
        }

        List<Item> itemsEnInventario = InventarioManager.Instance.ObtenerItems();

        foreach (var item in itemsDisponibles)
        {
            if (item.categoria == categoriaActual && !itemsEnInventario.Contains(item))
            {
                GameObject nuevoSlot = Instantiate(prefabSlotTienda, contenedorItems);
                UITiendaSlot slot = nuevoSlot.GetComponent<UITiendaSlot>();
                slot.Configurar(item, ComprarItem);
            }
        }
    }

    private void ComprarItem(Item item)
    {
        if (EconomyManager.instance.ObtenerDinero() >= item.precio)
        {
            EconomyManager.instance.RestarDinero(item.precio);
            InventarioManager.Instance.AgregarItemSinAbrir(item);

            ActualizarUI();
        }
        else
        {
            Debug.Log("No tienes suficiente dinero para comprar " + item.nombre);
        }
    }

}