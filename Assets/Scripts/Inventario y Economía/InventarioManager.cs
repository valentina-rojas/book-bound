using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private Transform contenedorItems;
    [SerializeField] private GameObject prefabSlotItem;
    [SerializeField] private Button botonAbrirInventario;

    private List<Item> items = new List<Item>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelInventario.SetActive(false);

        if (botonAbrirInventario != null)
        {
            botonAbrirInventario.onClick.AddListener(AbrirInventario);
        }
    }

    public void OcultarBotonAbrirInventario()
    {
        if (botonAbrirInventario != null)
        {
            botonAbrirInventario.gameObject.SetActive(false);
        }
    }

    public void MostrarBotonAbrirInventario()
    {
        if (botonAbrirInventario != null)
        {
            botonAbrirInventario.gameObject.SetActive(true);
        }
    }

    public void OcultarInventarioCompleto()
    {
        if (botonAbrirInventario != null)
            botonAbrirInventario.gameObject.SetActive(false);

        if (panelInventario != null)
            panelInventario.SetActive(false);
    }

    public void MostrarInventarioCompleto()
    {
        if (botonAbrirInventario != null)
            botonAbrirInventario.gameObject.SetActive(true);
    }

    public void AgregarItem(Item item, ItemMundo itemMundo)
    {
        items.Add(item);
        ActualizarUI();
        AbrirInventario();
    }

    private void ActualizarUI()
    {
        foreach (Transform child in contenedorItems)
        {
            Destroy(child.gameObject);
        }

        foreach (Item i in items)
        {
            GameObject nuevoSlot = Instantiate(prefabSlotItem, contenedorItems);
            UISlotItem slotItem = nuevoSlot.GetComponent<UISlotItem>();

            if (slotItem != null)
            {
                slotItem.Configurar(i, UsarItem);
            }
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
            }
        }
        else
        {
            var instancias = ItemMundo.ObtenerInstancias(item);
            if (instancias.Count > 0)
            {
                instancias[0].ReactivarEnMundo();
                items.Remove(item);
                ActualizarUI();
            }
        }
    }

    private SlotLugar EncontrarSlot(CategoriaItem categoria)
    {
        SlotLugar[] slots = FindObjectsByType<SlotLugar>(FindObjectsSortMode.None);
        foreach (var s in slots)
        {
            if (s.categoriaSlot == categoria)
                return s;
        }
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

    public void CerrarInventarioDesdeBoton()
    {
        CerrarInventario();
    }
    
    public List<Item> ObtenerItems()
    {
        return new List<Item>(items); 
    }

}