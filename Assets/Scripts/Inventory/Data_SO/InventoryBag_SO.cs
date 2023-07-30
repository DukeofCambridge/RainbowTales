using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory_SO/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemsofInventory;
    public InventoryItem GetInventoryItem(int ID)
    {
        return itemsofInventory.Find(i => i.itemID == ID);
    }
}
