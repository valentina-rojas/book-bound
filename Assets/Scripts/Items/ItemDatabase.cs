using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<Item> todosLosItems;

    private void Awake() => Instance = this;

    public Item GetItemByID(string id)
    {
        return todosLosItems.Find(i => i.nombre == id);
    }

}
