using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory_SO/BluePrintDataList")]
public class BluePrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;

    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrintDataList.Find(b => b.ID == itemID);
    }
}


[System.Serializable]
public class BluePrintDetails
{
    public int ID;
    public InventoryItem[] resourceItem = new InventoryItem[3];
    public GameObject buildPrefab;
}