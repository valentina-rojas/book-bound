using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    [Header("Configuración del pulso")]
    public float scaleSpeed = 2f;   
    public float scaleAmount = 0.1f; 

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = originalScale; 
    }

    private void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}