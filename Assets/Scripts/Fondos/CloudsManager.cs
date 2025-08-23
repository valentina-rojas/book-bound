using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsManager : MonoBehaviour
{
    public float velocidadMovimiento = 0.5f; 
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

    }

    void Update()
    {
        float nuevoPosX = Mathf.Repeat(Time.time * velocidadMovimiento, 30f); 
        transform.position = startPosition + Vector3.right * nuevoPosX;
    }
   
}
   