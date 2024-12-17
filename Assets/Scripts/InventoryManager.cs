using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    
    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log(item.itemName + " added to inventory.");
    }
    
    public void RemoveItem(Item item)
    {
        items.Remove(item);
        Debug.Log(item.itemName + " removed from inventory.");
    }
}
