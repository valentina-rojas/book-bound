using UnityEngine;
using System.Collections.Generic;

public class PageData : MonoBehaviour
{
    public int pageID;
    public PageCategory category;
    
    [HideInInspector] public Transform originalParent;

    public static List<PageData> todasLasPaginas = new List<PageData>();

    private void Awake()
    {
        originalParent = transform.parent;
        if (!todasLasPaginas.Contains(this))
        {
            todasLasPaginas.Add(this);
        }
    }

    private void OnDestroy()
    {
        if (todasLasPaginas.Contains(this))
        {
            todasLasPaginas.Remove(this);
        }
    }

    private void OnEnable()
    {
        if (!todasLasPaginas.Contains(this))
        {
            todasLasPaginas.Add(this);
        }
    }

    private void OnDisable()
    {
    }
}