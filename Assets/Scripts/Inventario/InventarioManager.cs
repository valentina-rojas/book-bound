using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private Transform contenedorItems;
    [SerializeField] public GameObject prefabSlotItem;
    [SerializeField] private Button botonAbrirInventario;
    public void OcultarBotonAbrirInventario() => botonAbrirInventario?.gameObject.SetActive(false);
    public void MostrarBotonAbrirInventario() => botonAbrirInventario?.gameObject.SetActive(true);
    public void MostrarInventarioCompleto() => MostrarBotonAbrirInventario();
    private List<Item> items = new List<Item>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelInventario.SetActive(false);

        if (botonAbrirInventario != null)
            botonAbrirInventario.onClick.AddListener(AbrirInventario);
    }

    public void OcultarInventarioCompleto()
    {
        OcultarBotonAbrirInventario();
        panelInventario?.SetActive(false);
    }

    public void AgregarItem(Item item, ItemMundo itemMundo)
    {
        items.Add(item);
        ActualizarUI();
        AbrirInventario(); 
    }

    public void AgregarItemSinAbrir(Item item)
    {
        items.Add(item);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        foreach (Transform child in contenedorItems)
            Destroy(child.gameObject);

        foreach (Item i in items)
        {
            GameObject nuevoSlot = Instantiate(prefabSlotItem, contenedorItems);
            UISlotItem slotItem = nuevoSlot.GetComponent<UISlotItem>();
            if (slotItem != null) slotItem.Configurar(i, UsarItem);
        }
    }

    public void UsarItem(Item item)
    {
        if (item.categoria == CategoriaItem.Paredes || item.categoria == CategoriaItem.Pisos)
        {
            SlotLugar slot = EncontrarSlot(item.categoria);
            if (slot != null)
            {
                slot.ColocarItem(item);
                items.Remove(item);
                ActualizarUI();
                return;
            }
        }

        var instancias = ItemMundo.ObtenerInstancias(item);
        if (instancias.Count > 0)
        {
            ItemMundo instancia = instancias.Find(x => !x.gameObject.activeSelf) ?? instancias[0];
            instancia.ReactivarEnMundo();
            items.Remove(item);
            ActualizarUI();
        }
    }

    public void EliminarItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            ActualizarUI();
        }
    }

    private SlotLugar EncontrarSlot(CategoriaItem categoria)
    {
        SlotLugar[] slots = FindObjectsByType<SlotLugar>(FindObjectsSortMode.None);
        foreach (var s in slots)
            if (s.categoriaSlot == categoria)
                return s;

        return null;
    }

    public void AbrirInventario()
    {
        panelInventario.SetActive(true);
        OcultarBotonAbrirInventario();
    }

    public void CerrarInventario()
    {
        panelInventario.SetActive(false);
        MostrarBotonAbrirInventario();
    }

    public void CerrarInventarioDesdeBoton() => CerrarInventario();

    public List<Item> ObtenerItems() => new List<Item>(items);
}
