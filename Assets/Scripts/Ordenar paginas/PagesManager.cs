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

    public void ActivarCategoriaCorrecta()
    {
        var personaje = GameManager.instance.personajeActual;
        if (personaje == null) return;

        PageCategory categoria = personaje.categoriaLibroReparar;

        PageData[] todasLasPaginas = FindObjectsByType<PageData>(FindObjectsSortMode.None);
        foreach (PageData pagina in todasLasPaginas)
        {
            pagina.gameObject.SetActive(pagina.category == categoria);
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
                Debug.Log("Faltan páginas visibles en un slot.");
                return;
            }

            if (pageData.pageID != slot.expectedPageID)
            {
                Debug.Log("Página fuera de lugar.");
                return;
            }
        }

        Debug.Log("¡Libro restaurado correctamente!");

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
}