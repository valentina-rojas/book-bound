using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cinematica : MonoBehaviour
{
    public Image imagenUI;             
    public Sprite[] imagenes;        
    public float tiempoPorImagen = 2f; 

    private int indiceActual = 0;

    void Start()
    {
        if (imagenes.Length > 0 && imagenUI != null)
        {
            StartCoroutine(ReproducirCinematica());
        }
        else
        {
            Debug.LogWarning("Faltan referencias en la cinemática.");
        }
    }

    IEnumerator ReproducirCinematica()
    {
        while (indiceActual < imagenes.Length)
        {
            imagenUI.sprite = imagenes[indiceActual];
            yield return new WaitForSeconds(tiempoPorImagen);
            indiceActual++;
        }

        Debug.Log("Cinemática finalizada.");
        CerrarCinematica();
    }

    public void CerrarCinematica()
    {
           ChangeScene("Gameplay");
    }


    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}

