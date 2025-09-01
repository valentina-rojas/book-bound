using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UISlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icono;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform panelInventarioRect;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private Item currentItem;
    private Action<Item> onItemClick;

    public void SetItem(Item item)
    {
        currentItem = item;
        if (item != null && icono != null)
        {
            icono.sprite = item.icono;
            icono.enabled = true;
        }
        else icono.enabled = false;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Configurar(Item item, Action<Item> onClickCallback)
    {
        SetItem(item);
        onItemClick = onClickCallback;
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onItemClick?.Invoke(currentItem));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (ObtenerSlotCuadroValidoDebajo() != null)
            icono.color = Color.white;
        else
            icono.color = new Color(1f, 1f, 1f, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        SlotCuadro slot = ObtenerSlotCuadroValidoDebajo();

        if (slot != null)
        {
            slot.ColocarItem(currentItem, this);
            InventarioManager.Instance.EliminarItem(currentItem);
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }

        icono.color = Color.white;
    }

    private SlotCuadro ObtenerSlotCuadroValidoDebajo()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponent<SlotCuadro>();
            if (slot != null) return slot;
        }
        return null;
    }
}