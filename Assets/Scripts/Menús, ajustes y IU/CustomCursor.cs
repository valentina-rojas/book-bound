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
            DontDestroyOnLoad(transform.root.gameObject); 
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
    }

    void Start()
    {
        Cursor.visible = false; 
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        GetComponent<RectTransform>().position = mousePos + offset;
    }
}
