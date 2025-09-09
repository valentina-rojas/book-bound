using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SonidoAjustes : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Icono del botón de sonido")]
    public Image botonIcono;
    public Sprite iconoSonidoOn;
    public Sprite iconoSonidoOff;

    [Header("Sliders de volumen (opcionales)")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool sonidoActivo = true;

    void Start()
    {
        sonidoActivo = PlayerPrefs.GetInt("SonidoActivo", 1) == 1;
        audioMixer.SetFloat("MasterVolume", sonidoActivo ? 0f : -80f);

        if (botonIcono != null)
            botonIcono.sprite = sonidoActivo ? iconoSonidoOn : iconoSonidoOff;

        if (musicSlider != null)
        {
            musicSlider.value = sonidoActivo ? 0.5f : 0f;
            musicSlider.onValueChanged.AddListener((value) =>
            {
                SetMusicVolume(value);
                VerificarReactivacionSonido(value);
            });
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sonidoActivo ? 0.5f : 0f;
            sfxSlider.onValueChanged.AddListener((value) =>
            {
                SetSFXVolume(value);
                VerificarReactivacionSonido(value);
            });
        }
    }


    public void SetMusicVolume(float volume)
    {
        float minVolume = 0.0001f;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, minVolume)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        float minVolume = 0.0001f;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, minVolume)) * 20);
    }

    public void ToggleSonido()
    {
        if (sonidoActivo)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
            sonidoActivo = false;
            if (botonIcono != null)
                botonIcono.sprite = iconoSonidoOff;
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", 0f);
            sonidoActivo = true;
            if (botonIcono != null)
                botonIcono.sprite = iconoSonidoOn;
        }

        PlayerPrefs.SetInt("SonidoActivo", sonidoActivo ? 1 : 0);
        PlayerPrefs.Save();
    }


    private void VerificarReactivacionSonido(float valor)
    {
        if (!sonidoActivo && valor > 0.01f)
        {
            audioMixer.SetFloat("MasterVolume", 0f);
            sonidoActivo = true;
            PlayerPrefs.SetInt("SonidoActivo", 1);
            PlayerPrefs.Save();

            if (botonIcono != null)
                botonIcono.sprite = iconoSonidoOn;
        }
    }
}