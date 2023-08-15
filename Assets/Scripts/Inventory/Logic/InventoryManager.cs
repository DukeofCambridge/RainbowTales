using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Items;
using Rainbow.Save;
using UnityEngine;

namespace Rainbow.Inventory
{
    //TODO: Infinite scrolling backpack
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataListSo;
        [Header("背包数据")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;
        private InventoryBag_SO _currentBoxBag;
        [Header("交易")]
        public int playerMoney;
        [Header("建造蓝图")]
        public BluePrintDataList_SO bluePrintData;
        [Header("箱子数据")]
        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;
        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        private void Start()
        {        
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        private void OnStartNewGameEvent(int obj)
        {
            playerBag = Instantiate(playerBagTemp);
            playerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            _currentBoxBag = bag_SO;
        }
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            //RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }
        private void OnDropItemEvent(int arg1, Vector3 arg2, ItemType arg3)
        {
            RemoveItem(arg1,1);
        }
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);

            //更新UI
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
                if (currentItem.itemID != targetItem.itemID)
                {
                    playerBag.itemsofInventory[fromIndex] = targetItem;
                    playerBag.itemsofInventory[targetIndex] = currentItem;
                }
                else
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    playerBag.itemsofInventory[targetIndex] = targetItem;
                    playerBag.itemsofInventory[fromIndex] = new InventoryItem();
                }
            }
            else
            {
                playerBag.itemsofInventory[targetIndex] = currentItem;
                playerBag.itemsofInventory[fromIndex] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        /// <summary>
        /// swap items between different bags
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //有不相同的两个物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //相同的两个物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //目标空格子
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }
        /// <summary>
        /// 根据位置返回背包数据列表
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemsofInventory,
                InventoryLocation.Box => _currentBoxBag.itemsofInventory,
                _ => null
            };
        }
        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (playerBag.itemsofInventory[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemsofInventory[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemsofInventory[index] = item;
            }
            else if (playerBag.itemsofInventory[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemsofInventory[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails">物品信息</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">是否卖东西</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //卖
            {
                if (playerBag.itemsofInventory[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //卖出总价
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
                else
                {
                    ShowMessage.Instance.Show("持有数量不够");
                }
            }
            else if (playerMoney - cost >= 0)   //买
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                    playerMoney -= cost;
                }
                else
                {
                    ShowMessage.Instance.Show("背包已满");
                }
                
            }
            else   //钱不够
            {
                ShowMessage.Instance.Show("持有金币不够");
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
        /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID">图纸ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }
        /// <summary>
        /// 查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }
        /// <summary>
        /// 加入箱子数据字典
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.boxBagData.itemsofInventory);
            Debug.Log("加入箱子字典: "+key);
        }
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.playerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemsofInventory);

            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            playerMoney = saveData.playerMoney;
            playerBag = Instantiate(playerBagTemp);
            playerBag.itemsofInventory = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemsofInventory);
        }
    }
}