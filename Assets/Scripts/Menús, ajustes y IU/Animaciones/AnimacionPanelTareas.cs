using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimacionPanelTareas : MonoBehaviour
{
    public Image image;            // Image del panel o de un overlay
    public Sprite[] frames;        // Sprites de la animación de cierre
    public float framesPerSecond = 12f;

    private int currentFrame;
    private float timer;
    private bool isPlaying;
    private System.Action onAnimationComplete; // callback para desactivar el panel

    void Start()
    {
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

    public void ReproducirAnimacion(System.Action callback = null)
    {
        onAnimationComplete = callback;
        currentFrame = 0;
        timer = 0f;
        isPlaying = true;
        image.sprite = frames[currentFrame];
        image.gameObject.SetActive(true);
    }

    private void FinalizarAnimacion()
    {
        isPlaying = false;
        image.gameObject.SetActive(false);

        // Llamar al callback cuando la animación termina
        if (onAnimationComplete != null)
            onAnimationComplete.Invoke();
    }
}
