using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UITiendaSlot : MonoBehaviour
{
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text textoNombre;
    [SerializeField] private TMP_Text textoPrecio;
    [SerializeField] private Button botonComprar;

    private Item currentItem;
    private Action<Item> onBuyClick;

    public void Configurar(Item item, Action<Item> onClickCallback)
    {
        currentItem = item;
        onBuyClick = onClickCallback;
        icono.sprite = item.iconoTienda != null ? item.iconoTienda : item.icono;
        textoNombre.text = item.nombre;
        textoPrecio.text = $"${item.precio}";
        botonComprar.onClick.RemoveAllListeners();
        botonComprar.onClick.AddListener(() => onBuyClick?.Invoke(currentItem));
    }
}