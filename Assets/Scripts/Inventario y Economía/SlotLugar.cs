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
        if (render != null && nuevoItem.icono != null)
            render.sprite = nuevoItem.icono;
        if (anterior != null)
            InventarioManager.Instance.AgregarItem(anterior, null);
    }
}