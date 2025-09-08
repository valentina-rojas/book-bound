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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enInventario && gameObject.activeSelf)
        {
            foreach (var instancia in itemsPorTipo[itemData])
            {
                instancia.gameObject.SetActive(false);
                instancia.enInventario = true;
            }
        }
    }
}