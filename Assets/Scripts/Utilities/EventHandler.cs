using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;      
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)      
    {          
        UpdateInventoryUI?.Invoke(location, list);      
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int id, Vector3 position)
    {
        InstantiateItemInScene?.Invoke(id, position);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;

    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool selected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, selected);
    }
}
