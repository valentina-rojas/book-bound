using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class Item : ScriptableObject
{
    public string nombre;
    public Sprite icono;
}
