using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfSlots : MonoBehaviour, IDropHandler
{
    public string generoPermitido;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (transform.childCount != 0)
        {
            GameObject current = transform.GetChild(0).gameObject;
            DraggableItem currentDraggable = current.GetComponent<DraggableItem>();
            currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
        }

        draggableItem.parentAfterDrag = transform;

        BookData bookData = dropped.GetComponent<BookData>();
        if (bookData != null)
        {
            if (bookData.tipoLibro == generoPermitido)
            {
                Debug.Log("Libro colocado correctamente.");
                if (ShelfManager.instance.audioLibroCorrecto != null && ShelfManager.instance.audioLibroCorrecto.clip != null)
                {
                    ShelfManager.instance.audioLibroCorrecto.PlayOneShot(ShelfManager.instance.audioLibroCorrecto.clip);
                }
            }

            else
            {
                Debug.Log("Este libro no corresponde a este estante.");
            }
        }

        ShelfEstante estante = GetComponentInParent<ShelfEstante>();
        if (estante != null)
            estante.VerificarEstante();

        ShelfManager.instance.RevisarOrganizacionConDelay();
    }
}

