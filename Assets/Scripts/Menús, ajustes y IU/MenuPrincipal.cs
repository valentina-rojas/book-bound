using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{

    public void IniciarJuego()
    {
        ChangeScene("Cinematica"); 
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    
}
