using UnityEngine;

[System.Serializable]
public class Option
{
    public string localizationKey = null;
    public bool correct = false;
    
    
    [HideInInspector]
    public string text;
}