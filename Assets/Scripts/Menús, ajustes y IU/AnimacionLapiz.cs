 using UnityEngine;
using UnityEngine.UI;

public class AnimacionLapiz : MonoBehaviour
{
    public Image image;
    public Sprite[] frames;

    private int currentFrame;
    public float framesPerSecond = 12f;
    private float timer;
    private bool isPlaying = false;

    void Start()
    {
        image.gameObject.SetActive(false); // Empieza desactivado
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


    public void ReproducirAnimacion()
    {
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
    }
}