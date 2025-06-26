using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SonidoAjustes : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixer audioMixer;

    void Start()
    {
        musicSlider.value = 0.5f;
        SetMusicVolume(musicSlider.value);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        sfxSlider.value = 0.5f;
        SetSFXVolume(sfxSlider.value);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
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

}

