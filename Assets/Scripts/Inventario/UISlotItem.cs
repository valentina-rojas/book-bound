using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UISlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icono;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform panelInventarioRect;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Item currentItem;
    private Action<Item> onItemClick;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Configurar(Item item, Action<Item> onClickCallback)
    {
        currentItem = item;
        onItemClick = onClickCallback;

        if (item != null && item.icono != null)
        {
            icono.sprite = item.icono;
            icono.enabled = true;
            icono.preserveAspect = true;
        }
        else
        {
            icono.enabled = false;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        onItemClick?.Invoke(currentItem);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!RectTransformUtility.RectangleContainsScreenPoint(
            panelInventarioRect, Input.mousePosition, canvas.worldCamera))
        {
            InventarioManager.Instance.UsarItem(currentItem);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}