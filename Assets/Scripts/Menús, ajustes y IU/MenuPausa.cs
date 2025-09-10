using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPausa : MonoBehaviour
{
    public static MenuPausa instance;
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject panelConfirmacionSalida;
    [SerializeField] private GameObject botonConfirmarSalida;
    [SerializeField] private GameObject botonCancelarSalida;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;

        SaveData data = SaveManager.CargarNivel();
        if (data != null && data.nivelActual > 1)
        {
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Salir()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void OcultarBotonPausa()
    {
        botonPausa.SetActive(false);
    }
    
    public void MostrarBotonPausa()
    {
        botonPausa.SetActive(true);
    }

}