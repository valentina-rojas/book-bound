using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BookData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int libroID;
    public string tipoLibro;
    public string titulo;
    public string descripcion;
    public Sprite imagenLibro;

    private Image image;
    private Color originalColor;

    private void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BookManager.instance.MostrarInformacion(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(originalColor.r * 0.7f, originalColor.g * 0.7f, originalColor.b * 0.7f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }
}
