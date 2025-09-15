using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfSlotsDecorativos : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem == null) return;

        Transform slotOrigen = draggableItem.parentAfterDrag;

        if (transform.childCount > 0)
        {
            GameObject current = transform.GetChild(0).gameObject;
            DraggableItem currentDraggable = current.GetComponent<DraggableItem>();
            if (currentDraggable != null && slotOrigen != null)
            {
                currentDraggable.transform.SetParent(slotOrigen);
                currentDraggable.transform.localPosition = Vector3.zero;
                currentDraggable.parentAfterDrag = slotOrigen; 
            }
        }

        draggableItem.transform.SetParent(transform);
        draggableItem.transform.localPosition = Vector3.zero;
        draggableItem.transform.localRotation = Quaternion.identity;
        draggableItem.transform.localScale = Vector3.one;
        draggableItem.parentAfterDrag = transform; 

    }
}

