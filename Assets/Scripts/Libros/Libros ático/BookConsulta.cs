using UnityEngine;
using UnityEngine.EventSystems;

public class BookConsulta : MonoBehaviour, IPointerClickHandler
{
    [TextArea(3, 10)] public string[] paginas;
    public string titulo;

    public void OnPointerClick(PointerEventData eventData)
    {
        BookConsultaManager.instance.AbrirLibro(this);
    }
}