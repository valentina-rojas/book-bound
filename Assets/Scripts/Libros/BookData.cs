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

    #region Sistema de Guardado
    public LibroGuardado ToLibroGuardado()
    {
        return new LibroGuardado()
        {
            libroID = this.libroID,
            tipoLibro = this.tipoLibro,
            titulo = this.titulo,
            descripcion = this.descripcion,
            estaHabilitado = this.gameObject.activeSelf,
            posicion = this.transform.localPosition, 
            parentPath = GetParentPath(this.transform.parent)
        };
    }

    private string GetParentPath(Transform parent)
    {
        if (parent == null) 
        {
            Debug.LogWarning("El libro no tiene padre asignado");
            return "";
        }
        
        return parent.name; 
    }

    public void FromLibroGuardado(LibroGuardado libroGuardado)
    {
        this.libroID = libroGuardado.libroID;
        this.tipoLibro = libroGuardado.tipoLibro;
        this.titulo = libroGuardado.titulo;
        this.descripcion = libroGuardado.descripcion;
        
        if (!string.IsNullOrEmpty(libroGuardado.parentPath))
        {
            ShelfSlots[] todosLosSlots = GameObject.FindObjectsOfType<ShelfSlots>(true);
            Transform parentSlot = System.Array.Find(todosLosSlots, s => s.gameObject.name == libroGuardado.parentPath)?.transform;
            
            if (parentSlot != null)
            {
                this.transform.SetParent(parentSlot);
                Debug.Log($"Libro {libroID} asignado a slot: {parentSlot.name}");
            }
            else
            {
                Debug.LogWarning($"No se encontró el slot: {libroGuardado.parentPath}");
            }
        }
        
        this.transform.localPosition = libroGuardado.posicion;
        this.gameObject.SetActive(libroGuardado.estaHabilitado);
        
        if (transform.parent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
    }
    #endregion
}