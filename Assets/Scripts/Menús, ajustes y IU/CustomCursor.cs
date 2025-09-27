using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Vector2 offset;
    private static CustomCursor instance;
    private RectTransform rectTransform;

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
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        rectTransform.position = mousePos + offset;
    }
}

