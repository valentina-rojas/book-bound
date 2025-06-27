using UnityEngine;
using UnityEngine.UI;

public class AnimacionFondoDonacion : MonoBehaviour
{
    public Image image;              
    public Sprite[] frames;      
    public float framesPerSecond = 12f;


    private int currentFrame = 0;
    private float timer = 0f;


    void Start()
    {
        if (frames.Length > 0 && image != null)
        {
            image.sprite = frames[0];
        }
    }


    void Update()
    {
        if (frames.Length == 0 || image == null) return;


        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
            timer = 0f;
        }
    }
}
