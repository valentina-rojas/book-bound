using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemMundo : MonoBehaviour, IPointerClickHandler
{
    public Item itemData; 
    public int cameraIndex; 
    public bool activoAlInicio = false; 
    
    private static Dictionary<Item, List<ItemMundo>> itemsPorTipo = new Dictionary<Item, List<ItemMundo>>();
    private bool enInventario = false;

    private void Awake()
    {
        if (!itemsPorTipo.ContainsKey(itemData))
        {
            itemsPorTipo[itemData] = new List<ItemMundo>();
        }
        itemsPorTipo[itemData].Add(this);
    }

    private void Start()
    {
        if (!activoAlInicio)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (itemsPorTipo.ContainsKey(itemData))
        {
            itemsPorTipo[itemData].Remove(this);
            if (itemsPorTipo[itemData].Count == 0)
            {
                itemsPorTipo.Remove(itemData);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enInventario && gameObject.activeSelf)
        {
            foreach (var instancia in itemsPorTipo[itemData])
            {
                instancia.gameObject.SetActive(false);
                instancia.enInventario = true;
            }
            
            InventarioManager.Instance.AgregarItem(itemData, this);
        }
    }

    public void ReactivarEnMundo()
    {
        int camIndex = CameraManager.instance.CurrentCameraIndex;
        
        foreach (var instancia in itemsPorTipo[itemData])
        {
            if (instancia.cameraIndex == camIndex)
            {
                instancia.gameObject.SetActive(true);
                instancia.enInventario = false;
                break;
            }
        }
    }

    public static List<ItemMundo> ObtenerInstancias(Item item)
    {
        if (itemsPorTipo.ContainsKey(item))
        {
            return itemsPorTipo[item];
        }
        return new List<ItemMundo>();
    }
}