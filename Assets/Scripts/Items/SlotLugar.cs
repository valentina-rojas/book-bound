using UnityEngine;

public class SlotLugar : MonoBehaviour
{
    public CategoriaItem categoriaSlot;
    public Item itemActual;
    public SpriteRenderer render;

    public void ColocarItem(Item nuevoItem)
    {
        Item anterior = itemActual;
        itemActual = nuevoItem;

        if (render != null && nuevoItem != null && nuevoItem.icono != null)
            render.sprite = nuevoItem.icono;
        else if (render != null)
            render.sprite = null;

        if (anterior != null)
            InventarioManager.Instance.AgregarItem(anterior, null);
    }

    public SlotGuardado ToSlotGuardado()
    {
        return new SlotGuardado()
        {
            slotPath = SaveManager.ObtenerRutaCompleta(transform),
            itemNombre = itemActual != null ? itemActual.nombre : null
        };
    }
}