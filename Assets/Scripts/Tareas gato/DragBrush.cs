using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragBrush : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Image image;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        posicionInicial = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Comenzó a arrastrarse el cepillo");
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Se soltó el cepillo y volvió a su posición");
        rectTransform.localPosition = posicionInicial;
        image.raycastTarget = true;
    }
}

