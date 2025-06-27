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

    public void CheckOrder()
    {
        foreach (PagesSlot slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                Debug.Log("Faltan páginas.");
                return;
            }

            PageData pageData = slot.transform.GetChild(0).GetComponent<PageData>();
            if (pageData.pageID != slot.expectedPageID)
            {
                Debug.Log("Página fuera de lugar.");
                return;
            }
        }

        Debug.Log("¡Libro restaurado correctamente!");

        foreach (PagesSlot slot in slots)
        {
            Transform page = slot.transform.GetChild(0);
            DraggableItem draggable = page.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                draggable.enabled = false;
            }
        }

         // Reproducir sonido de éxito
        if (audioSource != null && sonidoCorrecto != null)
        {
            audioSource.PlayOneShot(sonidoCorrecto);
        }

        //habilitar boton
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
