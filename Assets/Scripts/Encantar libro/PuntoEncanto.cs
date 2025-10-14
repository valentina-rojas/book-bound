using UnityEngine;
using UnityEngine.EventSystems;

public class PuntoEncanto : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public int indice;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (EncantoManager.instance != null)
            EncantoManager.instance.ComenzarDibujo(indice);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EncantoManager.instance != null)
            EncantoManager.instance.PasarPorPunto(indice);
    }
}
