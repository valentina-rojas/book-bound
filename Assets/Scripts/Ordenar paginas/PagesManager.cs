using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PagesManager : MonoBehaviour
{
    public static PagesManager instance;

    public PagesSlot[] slots;
    public Button botonEntregar;
    private CharacterSpawn characterSpawn;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        characterSpawn = FindFirstObjectByType<CharacterSpawn>();
        if (characterSpawn == null)
        {
            Debug.LogError("CharacterSpawn no encontrado por BookManager.");
        }
    }

    public void ResetSistema()
    {
        foreach (PageData pagina in PageData.todasLasPaginas)
        {
            if (pagina == null) continue;

            pagina.gameObject.SetActive(true);

            DraggableItem draggable = pagina.GetComponent<DraggableItem>();
            if (draggable != null)
                draggable.enabled = true;

            if (pagina.originalParent != null)
            {
                pagina.transform.SetParent(pagina.originalParent);
                pagina.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogWarning($"Página {pagina.pageID} no tiene originalParent asignado");
            }
        }

        foreach (PagesSlot slot in slots)
        {
            foreach (Transform child in slot.transform)
            {
                PageData pageData = child.GetComponent<PageData>();
                if (pageData != null && pageData.originalParent != null)
                {
                    child.SetParent(pageData.originalParent);
                    child.localPosition = Vector3.zero;
                }
            }
        }

        if (botonEntregar != null)
            botonEntregar.gameObject.SetActive(false);
    }

    public void ActivarCategoriaCorrecta()
    {
        var personaje = GameManager.instance.personajeActual;
        if (personaje == null) 
        {
            Debug.LogWarning("No hay personaje actual para activar categoría");
            return;
        }

        PageCategory categoria = personaje.categoriaLibroReparar;
        Debug.Log($"Activando categoría: {categoria} para personaje: {personaje.name}");

        foreach (PageData pagina in PageData.todasLasPaginas)
        {
            if (pagina == null) continue;
            
            bool debeEstarActiva = (pagina.category == categoria);
            pagina.gameObject.SetActive(debeEstarActiva);
            
            if (debeEstarActiva)
                Debug.Log($"Activando página ID: {pagina.pageID}, Categoría: {pagina.category}");
        }

        foreach (PagesSlot slot in slots)
        {
            slot.ActivarImagenPorCategoria(categoria);
        }
    }

    public void CheckOrder()
    {
        var personaje = GameManager.instance.personajeActual;
        if (personaje == null) return;

        PageCategory categoria = personaje.categoriaLibroReparar;

        foreach (PagesSlot slot in slots)
        {
            if (!slot.gameObject.activeInHierarchy) 
                continue; 

            PageData pageData = null;
            foreach (Transform child in slot.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    pageData = child.GetComponent<PageData>();
                    break;
                }
            }

            if (pageData == null)
            {
                return;
            }

            if (pageData.pageID != slot.expectedPageID)
            {
                return;
            }
        }

        foreach (PagesSlot slot in slots)
        {
            if (!slot.gameObject.activeInHierarchy) continue;

            Transform page = null;
            foreach (Transform child in slot.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    page = child;
                    break;
                }
            }

            if (page != null)
            {
                DraggableItem draggable = page.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    draggable.enabled = false;
                }
            }
        }

        if (audioSource != null && sonidoCorrecto != null)
        {
            audioSource.PlayOneShot(sonidoCorrecto);
        }

        botonEntregar.gameObject.SetActive(true);
    }

    public void FinalizarRestauracion()
    {
        CameraManager.instance.DesactivarPanelReparacion();
        GameManager.instance.CompletarRestauracion();

        if (characterSpawn != null)
        {
            characterSpawn.EndInteraction();
        }
    }

    public void DebugPaginas()
    {
        Debug.Log($"=== DEBUG PÁGINAS - Total: {PageData.todasLasPaginas.Count} ===");
        foreach (PageData pagina in PageData.todasLasPaginas)
        {
            if (pagina != null)
            {
                Debug.Log($"Página {pagina.pageID} - Categoría: {pagina.category} - Activa: {pagina.gameObject.activeSelf} - Parent: {pagina.transform.parent.name}");
            }
            else
            {
                Debug.Log("Página nula en la lista");
            }
        }
    }

    private void OnDestroy()
    {
        PageData.todasLasPaginas.Clear();
    }
}