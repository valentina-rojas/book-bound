using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BookCoverManager : MonoBehaviour
{
    public GameObject portadaEditable;
    public GameObject portadaFinal;
    public Button finalizarButton;
    public RectTransform areaPortada;
    public TMP_Text textoTituloLibro;

    public void ActualizarTituloLibro()
    {
        var personaje = GameManager.instance.personajeActual;
        if (personaje != null && textoTituloLibro != null)
        {
            StartCoroutine(personaje.GetTituloLibroPortadaLocalized((textoLocalizado) =>
            {
                textoTituloLibro.text = textoLocalizado;
            }));
        }
        else
        {
            textoTituloLibro.text = "";
        }
    }
    
    public void ActivarStickersPorSet()
    {
        var personaje = GameManager.instance.personajeActual;
        if (personaje == null) return;

        StickerSet setDeseado = personaje.setStickersDeseado;

        foreach (Transform child in portadaEditable.transform)
        {
            StickerData data = child.GetComponent<StickerData>();
            if (data != null)
            {
                child.gameObject.SetActive(data.stickerSet == setDeseado);
            }
        }
    }


    public void VerificarElementosEnPortada()
    {
        finalizarButton.interactable = portadaEditable.transform.childCount > 0;
    }

    public void Finalizar()
    {
        portadaFinal.SetActive(true);

        List<StickerID> stickersUsados = new List<StickerID>();

        foreach (Transform child in portadaEditable.transform)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(areaPortada, RectTransformUtility.WorldToScreenPoint(null, child.position)))
            {
                StickerData data = child.GetComponent<StickerData>();
                if (data != null && !stickersUsados.Contains(data.stickerID))
                {
                    stickersUsados.Add(data.stickerID);
                }

                DraggableItem draggable = child.GetComponent<DraggableItem>();
                if (draggable != null)
                    draggable.enabled = false;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        GameManager.instance.CompletarPortada(stickersUsados);
    }
}