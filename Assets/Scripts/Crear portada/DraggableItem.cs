using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private Canvas canvas;
    public Image image;

    [Header("Opcional: límite de arrastre")]
    public RectTransform limiteArrastre;

    private RectTransform rectTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        if (limiteArrastre != null)
        {
            Vector2 limitSize = limiteArrastre.rect.size;
            Vector2 itemSize = rectTransform.rect.size;

            Vector2 halfLimit = limitSize / 2f;
            Vector2 halfItem = itemSize / 2f;

            float minX = -halfLimit.x + halfItem.x;
            float maxX = halfLimit.x - halfItem.x;
            float minY = -halfLimit.y + halfItem.y;
            float maxY = halfLimit.y - halfItem.y;

            float clampedX = Mathf.Clamp(localPoint.x, minX, maxX);
            float clampedY = Mathf.Clamp(localPoint.y, minY, maxY);

            transform.localPosition = new Vector2(clampedX, clampedY);
        }
        else
        {
            transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
