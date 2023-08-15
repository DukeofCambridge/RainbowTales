using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainbow.Items;
using UnityEngine;

namespace Rainbow.Inventory
{
    // 注意，Box的prefab是有furniture组件的，因为建造box后需要在重新加载场景时刷新重建，但场景中一开始就有的Box是不需要furniture组件的！它们只需要一开始就加入box字典中
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;

        public GameObject mouseIcon;
        private bool canOpen = false;
        private bool isOpen;
        public int index;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void Start()
        {
            InitBox(index);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
            {
                //打开箱子
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }

            if (!canOpen && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }

            if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        /// <summary>
        /// 初始化Box和数据
        /// </summary>
        /// <param name="boxIndex"></param>
        public void InitBox(int boxIndex)
        {
            index = boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)  //刷新地图读取数据
            {
                Debug.Log("拿到箱子数据"+key);
                boxBagData.itemsofInventory = InventoryManager.Instance.GetBoxDataList(key);
            }
            else     //新建箱子
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }
    }
}
