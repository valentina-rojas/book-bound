using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BookCoverManager : MonoBehaviour
{
    public GameObject portadaEditable;
    public Button finalizarButton;
    public RectTransform areaPortada;
    public TMP_Text textoTituloLibro;

    [Header("Portadas por set")]
    public GameObject portadaDefault;
    public GameObject portadaAventura;
    public GameObject portadaAstronomico;

    public IEnumerator ActualizarTituloLibroDespuesDeFrame()
    {
        yield return null;

        var personaje = GameManager.instance.personajeActual;
        if (personaje != null && textoTituloLibro != null)
        {
            yield return StartCoroutine(personaje.GetTituloLibroPortadaLocalized((textoLocalizado) =>
            {
                textoTituloLibro.text = textoLocalizado;
                textoTituloLibro.ForceMeshUpdate();
            }));
        }
        else if (textoTituloLibro != null)
        {
            textoTituloLibro.text = "";
            textoTituloLibro.ForceMeshUpdate();
        }
    }
    
    public void ActivarStickersPorSet()
    {
        if (finalizarButton != null)
        {
            finalizarButton.gameObject.SetActive(true);
            finalizarButton.interactable = true;
        }
        var personaje = GameManager.instance.personajeActual;
        if (personaje == null) return;

        StickerSet setDeseado = personaje.setStickersDeseado;
        
        ActualizarPortadaSegunSet(setDeseado);

        foreach (Transform child in portadaEditable.transform)
        {
            StickerData data = child.GetComponent<StickerData>();
            if (data != null)
            {
                child.gameObject.SetActive(data.stickerSet == setDeseado);
            }
        }
    }

    private void ActualizarPortadaSegunSet(StickerSet setDeseado)
    {
        portadaDefault?.SetActive(false);
        portadaAventura?.SetActive(false);
        portadaAstronomico?.SetActive(false);

        switch (setDeseado)
        {
            case StickerSet.Default:
                portadaDefault?.SetActive(true);
                break;
            case StickerSet.Aventura:
                portadaAventura?.SetActive(true);
                break;
            case StickerSet.Astronomico:
                portadaAstronomico?.SetActive(true);
                break;
        }
    }

    public void VerificarElementosEnPortada()
    {
        bool tieneElementos = portadaEditable.transform.childCount > 0;
    }

    public void Finalizar()
    {
        if (finalizarButton != null)
            finalizarButton.gameObject.SetActive(false);

        List<StickerID> stickersUsados = new List<StickerID>();

        foreach (Transform child in portadaEditable.transform)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                areaPortada, 
                RectTransformUtility.WorldToScreenPoint(null, child.position)))
            {
                StickerData data = child.GetComponent<StickerData>();
                if (data != null && !stickersUsados.Contains(data.stickerID))
                    stickersUsados.Add(data.stickerID);

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
        StartCoroutine(MostrarPreviewPortada());
    }

    private IEnumerator MostrarPreviewPortada()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.instance.DesctivarPanelPortada();
    }

}