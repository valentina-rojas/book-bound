using UnityEngine;
using UnityEngine.UI;
using System;

public class AnimacionBoton : MonoBehaviour
{
    public Image image;         // Imagen con los frames de la animación
    public Sprite[] frames;     // Frames de la animación
    public float framesPerSecond = 12f;

    private int currentFrame;
    private float timer;
    private bool isPlaying;
    private Action onAnimationComplete;

    void Start()
    {
        if (image != null)
            image.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            currentFrame++;
            if (currentFrame >= frames.Length)
            {
                FinalizarAnimacion();
                return;
            }

            image.sprite = frames[currentFrame];
            timer = 0f;
        }
    }

    public void ReproducirAnimacion(Action callback = null)
    {
        onAnimationComplete = callback;
        currentFrame = 0;
        timer = 0f;
        isPlaying = true;

        if (image != null)
        {
            image.sprite = frames[currentFrame];
            image.gameObject.SetActive(true);
        }
    }

    private void FinalizarAnimacion()
    {
        isPlaying = false;

        if (image != null)
            image.gameObject.SetActive(false);

        onAnimationComplete?.Invoke();
    }
}
