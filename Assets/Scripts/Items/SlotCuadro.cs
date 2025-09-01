using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCuadro : MonoBehaviour, IPointerClickHandler
{
    public Item itemActual;
    public Image render;
    public bool EstaOcupado => itemActual != null;

    public void ColocarItem(Item item, UISlotItem uiItem)
    {
        if (itemActual != null)
        {
            InventarioManager.Instance.AgregarItem(itemActual, null);
            uiItem.SetItem(item);
        }

        itemActual = item;
        if (render != null && item != null)
        {
            render.sprite = item.icono;
            render.color = Color.white;
        }
    }

    public void QuitarItem()
    {
        itemActual = null;
        if (render != null)
        {
            render.sprite = null;
            render.color = new Color(1f, 1f, 1f, 1f / 255f); 
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemActual != null)
        {
            InventarioManager.Instance.AgregarItem(itemActual, null);
            QuitarItem();
        }
    }
}