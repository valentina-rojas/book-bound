using UnityEngine;
using UnityEngine.UI;
using System;

public class UISlotItem : MonoBehaviour
{
    [SerializeField] private Image icono;
    [SerializeField] private Button button; 
    
    private Item currentItem;
    private Action<Item> onItemClick;

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
}