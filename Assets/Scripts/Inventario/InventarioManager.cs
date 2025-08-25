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
    private Dictionary<Item, ItemMundo> itemMundoReferences = new Dictionary<Item, ItemMundo>();

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

    public void AgregarItem(Item item, ItemMundo itemMundo)
    {
        items.Add(item);
        itemMundoReferences[item] = itemMundo;
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
        if (itemMundoReferences.ContainsKey(item))
        {
            itemMundoReferences[item].ReactivarEnMundo();
            items.Remove(item);
            itemMundoReferences.Remove(item);
            ActualizarUI();
        }
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
}