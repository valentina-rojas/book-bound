using UnityEngine;
using UnityEngine.EventSystems;

public class BookCoverDropArea : MonoBehaviour, IDropHandler
{
    public BookCoverManager coverManager; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            droppedObject.transform.SetParent(this.transform);
            coverManager.VerificarElementosEnPortada(); 
        }
    }
}
