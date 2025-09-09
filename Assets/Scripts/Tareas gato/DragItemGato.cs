using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemGato : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string tipoItem; 

    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Image image;
    private Transform parentOriginal;
    private bool estaArrastrando = false;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        posicionInicial = rectTransform.localPosition;
        parentOriginal = rectTransform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        rectTransform.SetParent(canvas.transform, true);
        estaArrastrando = true;
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

            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);

            switch (tipoItem)
            {
                case "cepillo":
                    TendCat.instance?.VerificarCepilladoConItem(screenPos);
                    break;
                case "comida":
                    TendCat.instance?.VerificarAlimentacionConItem(screenPos);
                    break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        estaArrastrando = false;
        rectTransform.SetParent(parentOriginal, true);
        rectTransform.localPosition = posicionInicial;
        rectTransform.rotation = Quaternion.identity;
        image.raycastTarget = true;

        if (tipoItem == "cepillo")
            TendCat.instance?.ReiniciarBarraCepillado();
    }
}