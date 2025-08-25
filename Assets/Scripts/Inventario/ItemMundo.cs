using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMundo : MonoBehaviour, IPointerClickHandler
{
    public Item itemData; 
    private bool enInventario = false;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    private void Start()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enInventario)
        {
            InventarioManager.Instance.AgregarItem(itemData, this);
            gameObject.SetActive(false);
            enInventario = true;
        }
    }

    public void ReactivarEnMundo()
    {
        gameObject.SetActive(true);
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;
        enInventario = false;
    }
}