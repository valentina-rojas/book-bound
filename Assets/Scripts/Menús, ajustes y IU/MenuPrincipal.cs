using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void NuevaPartida()
    {
        SaveManager.BorrarGuardado();
        SceneManager.LoadScene("Cinematica");
    }

    public void CargarPartida()
    {
        SaveData data = SaveManager.CargarNivel();

        if (data == null || data.nivelActual <= 1)
        {
            SceneManager.LoadScene("Cinematica");
        }
        else
        {
            SceneManager.LoadScene("Gameplay"); 
        }
    }

}
