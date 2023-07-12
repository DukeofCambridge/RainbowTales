using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rainbow.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemDataListSo;

        public ItemDetails GetItemDetails(int id)
        {
            return itemDataListSo.itemDetailsList.Find(i => i.itemID == id);
        }
    }
}