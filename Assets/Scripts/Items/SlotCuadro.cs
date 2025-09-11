using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotCuadro : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Slot")]
    public Item itemActual;
    public Image render;
    public bool EstaOcupado => itemActual != null;

    private UISlotItem dragItemInstance;
    private Canvas canvas;
    private SlotCuadro slotOrigen;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public bool TryColocarItem(Item item, out Item itemReemplazado)
    {
        itemReemplazado = null;

        if (item == null || item.categoria != CategoriaItem.Cuadros)
            return false;

        itemReemplazado = itemActual;
        itemActual = item;

        if (render != null && item != null)
        {
            render.sprite = item.icono;
            render.color = Color.white;
        }
        else if (render != null)
        {
            render.sprite = null;
            render.color = new Color(1f, 1f, 1f, 1f / 255f);
        }

        return true;
    }

    public void QuitarItem()
    {
        itemActual = null;
        if (render != null)
        {
            render.sprite = null;
            render.color = new Color(1f, 1f, 1f, 1f / 255f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemActual != null)
        {
            InventarioManager.Instance.AgregarItem(itemActual, null);
            QuitarItem();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemActual == null) return;

        slotOrigen = this;

        GameObject temp = Instantiate(InventarioManager.Instance.prefabSlotItem);
        dragItemInstance = temp.GetComponent<UISlotItem>();
        dragItemInstance.SetItem(itemActual);

        dragItemInstance.transform.SetParent(canvas.transform, false);
        dragItemInstance.transform.SetAsLastSibling();

        RectTransform rt = dragItemInstance.GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.pivot = new Vector2(0.5f, 0.5f);

        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out worldPos
        );
        rt.position = worldPos;

        QuitarItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragItemInstance == null) return;

        RectTransform rt = dragItemInstance.GetComponent<RectTransform>();
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out worldPos
        );
        rt.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragItemInstance == null) return;
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        SlotCuadro slotDestino = null;
        foreach (var r in results)
        {
            slotDestino = r.gameObject.GetComponent<SlotCuadro>();
            if (slotDestino != null) break;
        }

        if (slotDestino != null)
        {
            if (dragItemInstance.currentItem != null &&
                dragItemInstance.currentItem.categoria == CategoriaItem.Cuadros)
            {
                if (slotDestino.TryColocarItem(dragItemInstance.currentItem, out var itemEnDestino))
                {
                    if (itemEnDestino != null)
                        slotOrigen.TryColocarItem(itemEnDestino, out _);
                }
                else
                {
                    slotOrigen.TryColocarItem(dragItemInstance.currentItem, out _);
                }
            }
            else
            {
                InventarioManager.Instance.AgregarItem(dragItemInstance.currentItem, null);
            }
        }
        else
        {
            InventarioManager.Instance.AgregarItem(dragItemInstance.currentItem, null);
        }

        Destroy(dragItemInstance.gameObject);
        dragItemInstance = null;
        slotOrigen = null;
    }

    public SlotCuadroGuardado ToSlotCuadroGuardado()
    {
        return new SlotCuadroGuardado()
        {
            slotPath = SaveManager.ObtenerRutaCompleta(transform),
            itemNombre = itemActual != null ? itemActual.nombre : null
        };
    }
}