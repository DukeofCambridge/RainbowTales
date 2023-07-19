using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Items;
using UnityEngine;

namespace Rainbow.Inventory
{
    //TODO: Infinite scrolling backpack
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataListSo;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }

        public ItemDetails GetItemDetails(int id)
        {
            return itemDataListSo.itemDetailsList.Find(i => i.itemID == id);
        }

        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否要销毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {
            int index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            //Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemsofInventory.Count; i++)
            {
                if (playerBag.itemsofInventory[i].itemID == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>-1则没有这个物品否则返回序号</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemsofInventory.Count; i++)
            {
                if (playerBag.itemsofInventory[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())    //背包没有这个物品 同时背包有空位
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemsofInventory.Count; i++)
                {
                    if (playerBag.itemsofInventory[i].itemID == 0)
                    {
                        playerBag.itemsofInventory[i] = item;
                        break;
                    }
                }
            }
            else    //背包有这个物品
            {
                int currentAmount = playerBag.itemsofInventory[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemsofInventory[index] = item;
            }
        }
        /// <summary>
        /// swap items within a bag
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            //in fact the two pointers exchange the position they point to in memory space, not exchange the two memory areas
            InventoryItem currentItem = playerBag.itemsofInventory[fromIndex];
            InventoryItem targetItem = playerBag.itemsofInventory[targetIndex];
            if (targetItem.itemID != 0)
            {
                playerBag.itemsofInventory[fromIndex] = targetItem;
                playerBag.itemsofInventory[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemsofInventory[targetIndex] = currentItem;
                playerBag.itemsofInventory[fromIndex] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
    }
}