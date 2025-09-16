using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Vector2 offset;
    private static CustomCursor instance;

    void Awake()
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

    void Start()
    {
        Cursor.visible = false; 
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos + offset;
    }
}