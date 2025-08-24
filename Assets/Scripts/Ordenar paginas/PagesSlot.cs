using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PagesSlot : MonoBehaviour, IDropHandler
{
    public int expectedPageID;

    [Header("Placeholders por categoría")]
    public GameObject placeholderDefault;
    public GameObject placeholderRecetas;

    public void ActivarImagenPorCategoria(PageCategory categoria)
    {
        if (placeholderDefault != null) placeholderDefault.SetActive(categoria == PageCategory.Default);
        if (placeholderRecetas != null) placeholderRecetas.SetActive(categoria == PageCategory.Recetas);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (transform.childCount > 0)
        {
            Transform currentChild = null;

            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    currentChild = child;
                    break;
                }
            }

            if (currentChild != null)
            {
                DraggableItem currentDraggable = currentChild.GetComponent<DraggableItem>();
                Transform previousParent = draggableItem.parentAfterDrag;

                currentChild.SetParent(previousParent);
                currentChild.localPosition = Vector3.zero;
            }
        }

        draggableItem.parentAfterDrag = transform;
        dropped.transform.SetParent(transform);
        dropped.transform.localPosition = Vector3.zero;

        PageData pageData = dropped.GetComponent<PageData>();
        if (pageData.pageID == expectedPageID)
            Debug.Log("Página correcta");
        else
            Debug.Log("Página incorrecta");
        PagesManager.instance.CheckOrder();
    }
}