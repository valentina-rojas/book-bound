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
    
    [Header("Verificación de título")]
    public RectTransform areaTitulo; // Área donde está el título que no debe ser cubierto

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
        int extras = 0;

        foreach (Transform child in portadaEditable.transform)
        {
            // Verificar si el punto central está dentro del área (para considerar el sticker como "dentro")
            bool puntoCentralDentro = RectTransformUtility.RectangleContainsScreenPoint(
                areaPortada,
                RectTransformUtility.WorldToScreenPoint(null, child.position));

            if (puntoCentralDentro)
            {
                StickerData data = child.GetComponent<StickerData>();
                if (data != null)
                {
                    if (!stickersUsados.Contains(data.stickerID))
                        stickersUsados.Add(data.stickerID);

                    // Para el extra: verificar si está COMPLETAMENTE dentro
                    if (GameManager.instance.personajeActual != null &&
                        GameManager.instance.personajeActual.stickersRequeridos.Contains(data.stickerID) &&
                        EstaStickerCompletamenteDentro(child as RectTransform, areaPortada))
                    {
                        extras++;
                    }
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

        // Verificar si los stickers no cubren el título (3 extras adicionales)
        if (!StickersCubrenTitulo())
        {
            extras += 3;
            Debug.Log("+3 extras: Los stickers no cubren el título");
        }

        GameManager.instance.CompletarPortada(stickersUsados, extras);
        StartCoroutine(MostrarPreviewPortada());
    }

    private bool EstaStickerCompletamenteDentro(RectTransform sticker, RectTransform areaPortada)
    {
        if (sticker == null || areaPortada == null) return false;

        // Obtener las esquinas del sticker en espacio de pantalla
        Vector3[] stickerCorners = new Vector3[4];
        sticker.GetWorldCorners(stickerCorners);

        // Verificar que todas las esquinas del sticker estén dentro del área
        foreach (Vector3 corner in stickerCorners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, corner);
            if (!RectTransformUtility.RectangleContainsScreenPoint(areaPortada, screenPoint, null))
            {
                return false; // Una esquina está fuera del área
            }
        }

        return true; // Todas las esquinas están dentro del área
    }

    private bool StickersCubrenTitulo()
    {
        if (areaTitulo == null) return false;

        foreach (Transform child in portadaEditable.transform)
        {
            // Solo verificar stickers que están dentro del área de la portada
            bool puntoCentralDentro = RectTransformUtility.RectangleContainsScreenPoint(
                areaPortada,
                RectTransformUtility.WorldToScreenPoint(null, child.position));

            if (puntoCentralDentro)
            {
                StickerData data = child.GetComponent<StickerData>();
                if (data != null)
                {
                    // Verificar si este sticker se superpone con el área del título
                    if (StickerSuperponeTitulo(child as RectTransform, areaTitulo))
                    {
                        return true; // Al menos un sticker cubre el título
                    }
                }
            }
        }

        return false; // Ningún sticker cubre el título
    }

    private bool StickerSuperponeTitulo(RectTransform sticker, RectTransform areaTitulo)
    {
        if (sticker == null || areaTitulo == null) return false;

        // Obtener los rectángulos en espacio de mundo
        Rect stickerRect = GetWorldRect(sticker);
        Rect tituloRect = GetWorldRect(areaTitulo);

        // Verificar si los rectángulos se superponen
        return stickerRect.Overlaps(tituloRect);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        Vector2 min = corners[0];
        Vector2 max = corners[2];
        
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private IEnumerator MostrarPreviewPortada()
    {
        yield return new WaitForSeconds(2f);
        CameraManager.instance.DesctivarPanelPortada();
    }
}