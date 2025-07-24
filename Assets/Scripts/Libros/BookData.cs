using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using System.Collections;

public class BookData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Datos del libro
    public int libroID;
    public string tipoLibro;

    public string titulo;
    public string descripcion;

    public Sprite imagenLibro;
    #endregion

    #region Componentes internos
    private Image image;
    private Color originalColor;
    #endregion

    #region Unity Events
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
    #endregion

    #region Localización
    public IEnumerator GetTituloLocalized(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(titulo))
        {
            callback?.Invoke("");
            yield break;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "TitulosLibros",
            TableEntryReference = titulo
        };

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            callback?.Invoke(handle.Result);
        else
            callback?.Invoke(titulo);
    }

    public IEnumerator GetDescripcionLocalized(System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(descripcion))
        {
            callback?.Invoke("");
            yield break;
        }

        var localizedString = new LocalizedString
        {
            TableReference = "DescripcionLibros",
            TableEntryReference = descripcion
        };

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            callback?.Invoke(handle.Result);
        else
            callback?.Invoke(descripcion);
    }
    #endregion
}