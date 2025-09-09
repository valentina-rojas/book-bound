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

        if (EconomyManager.instance != null)
            EconomyManager.instance.ReiniciarDineroNivel(); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Salir()
    {
        panelConfirmacionSalida.SetActive(true);
    }

    public void ConfirmarSalida()
    {
        Time.timeScale = 1f;
        SaveManager.BorrarGuardado();

        if (EconomyManager.instance != null)
            EconomyManager.instance.ReiniciarDineroNivel();

        if (InventarioManager.Instance != null)
            InventarioManager.Instance.ObtenerItems().Clear();

        SceneManager.LoadScene("MenuPrincipal");
    }

    public void CancelarSalida()
    {
        panelConfirmacionSalida.SetActive(false);
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