using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Services.Analytics;
using static EventManager; 

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
            bool placedCorrect = (bookData.tipoLibro == generoPermitido);

            if (placedCorrect)
            {
                if (ShelfManager.instance.audioLibroCorrecto != null && ShelfManager.instance.audioLibroCorrecto.clip != null)
                {
                    ShelfManager.instance.audioLibroCorrecto.PlayOneShot(ShelfManager.instance.audioLibroCorrecto.clip);
                }
            }

            RegistrarEventoEstante(bookData, placedCorrect);
        }

        ShelfEstante estante = GetComponentInParent<ShelfEstante>();
        if (estante != null)
            estante.VerificarEstante();

        ShelfManager.instance.RevisarOrganizacionConDelay();
    }

    private void RegistrarEventoEstante(BookData bookData, bool placedCorrect)
    {
        EstanteEvent estanteEvent = new EstanteEvent();
        estanteEvent.bookId = bookData.libroID.ToString();
        estanteEvent.openedBefore = StaticVariables.SessionData.bookOpened;
        estanteEvent.placedCorrect = placedCorrect;
        estanteEvent.level = GameManager.instance.nivelActual;

#if !UNITY_EDITOR
    Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(estanteEvent);
#else
        Debug.Log($"[ANALYTICS] EstanteEvent: bookId={bookData.libroID}, openedBefore={StaticVariables.SessionData.bookOpened}, placedCorrect={placedCorrect}, level={GameManager.instance.nivelActual}");
#endif
        StaticVariables.SessionData.bookOpened = false;
    }

}