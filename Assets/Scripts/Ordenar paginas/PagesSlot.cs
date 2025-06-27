using UnityEngine;
using UnityEngine.EventSystems;

public class PagesSlot : MonoBehaviour, IDropHandler
{

    public int expectedPageID;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        // Si este slot ya tiene una página
        if (transform.childCount != 0)
        {
            // La página actual en este slot
            GameObject current = transform.GetChild(0).gameObject;
            DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

            // Guardar el parent actual de la página soltada
            Transform previousParent = draggableItem.parentAfterDrag;

            // Mover la página actual al slot anterior de la página soltada
            currentDraggable.transform.SetParent(previousParent);
            current.transform.localPosition = Vector3.zero; // Centramos por si acaso

        }

        // Colocar la página soltada en este slot
        draggableItem.parentAfterDrag = transform;
        dropped.transform.SetParent(transform);
        dropped.transform.localPosition = Vector3.zero;

        // Verificar si corresponde
        PageData pageData = dropped.GetComponent<PageData>();
        if (pageData.pageID == expectedPageID)
        {
            Debug.Log("corresponde");
        }
        else
        {
            Debug.Log("no corresponde a este estante");
        }

        // Chequear el orden total
        PagesManager.instance.CheckOrder();
    }
}