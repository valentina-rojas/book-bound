using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragRegadera : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Image image;

    [Header("Áreas de las plantas")]
    public RectTransform[] areasPlantas;

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

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);

            bool sobreAlgunaPlanta = false;

            foreach (var areaPlanta in areasPlantas)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(areaPlanta, screenPos, canvas.worldCamera))
                {
                    sobreAlgunaPlanta = true;
                    break;
                }
            }

            rectTransform.rotation = Quaternion.Euler(0, 0, sobreAlgunaPlanta ? 45f : 0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.localPosition = posicionInicial;
        rectTransform.rotation = Quaternion.Euler(0, 0, 0f);
        image.raycastTarget = true;
    }
}