using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sonidoLibroCorrecto;
    public AudioSource sonidoLibroIncorrecto;
    public AudioSource sonidoEstrellas;
    public AudioSource sonidoCampanilla;
    public AudioSource sonidoRegadera;
    public AudioSource sonidoRuidoSala;
    public AudioSource sonidoSeleccionarLibro;
    public AudioSource sonidoSilenciarSala;
    public AudioSource sonidoRespuestaTriviaCorrecta;
    public AudioSource sonidosonidoRespuestaTriviaIncorrecta;
    public AudioSource sonidoArpasLibroDonado;
    public AudioSource sonidoGato;
    public AudioSource sonidoRonroneo;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    
    }
}
