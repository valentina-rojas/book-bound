using UnityEngine;

public enum CategoriaItem
{
    Pisos,
    Paredes,
    Cuadros,
    Muebles,
    Plantas,
    Herramientas,
    Otros
}

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class Item : ScriptableObject
{
    public string nombre;
    public Sprite icono;
    public Sprite iconoTienda;
    public int precio;
    public CategoriaItem categoria;

    [HideInInspector] 
    public bool comprado = false; 
}