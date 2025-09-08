using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class UISlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icono;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform panelInventarioRect;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    public Item currentItem;
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
        icono.color = (ObtenerSlotCuadroValidoDebajo() != null) ? Color.white : new Color(1f, 1f, 1f, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        bool colocado = false;

        SlotCuadro slot = ObtenerSlotCuadroValidoDebajo();
        if (slot != null)
        {
            if (slot.TryColocarItem(currentItem, out var reemplazado))
            {
                InventarioManager.Instance.EliminarItem(currentItem);
                if (reemplazado != null)
                    InventarioManager.Instance.AgregarItemSinAbrir(reemplazado);
                colocado = true;
            }
        }

        if (!colocado && currentItem != null &&
            currentItem.categoria == CategoriaItem.Paredes || 
            currentItem.categoria == CategoriaItem.Pisos || 
            currentItem.categoria == CategoriaItem.Otros)
        {
            SlotLugar[] slotsLugar = GameObject.FindObjectsByType<SlotLugar>(FindObjectsSortMode.None);
            foreach (var s in slotsLugar)
            {
                if (s.categoriaSlot == currentItem.categoria)
                {
                    s.ColocarItem(currentItem);
                    InventarioManager.Instance.EliminarItem(currentItem);
                    colocado = true;
                    break;
                }
            }
        }

        if (!colocado)
            rectTransform.anchoredPosition = originalPosition;

        icono.color = Color.white;
    }

    private SlotCuadro ObtenerSlotCuadroValidoDebajo()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponent<SlotCuadro>();
            if (slot != null) return slot;
        }
        return null;
    }
}