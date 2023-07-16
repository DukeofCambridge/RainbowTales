using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public ItemTooltip itemTooltip;
    public Image dragItem;
    [SerializeField] private GameObject bagUI;
    private bool _bagOpened;
    [SerializeField] private SlotUI[] playerBagSlots;
    private void Start()
    {
        for (int i = 0; i < playerBagSlots.Length; ++i)
        {
            playerBagSlots[i].slotIndex = i;
        }

        _bagOpened = bagUI.activeInHierarchy;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenBagUI();
        }
    }

    private void OnEnable()
    {
        EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
    }

    private void OnDisable()
    {
        EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
    }

    private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        switch (location)
        {
            case InventoryLocation.Player:
                for (int i = 0; i < playerBagSlots.Length; ++i)
                {
                    if (list[i].itemAmount > 0)
                    {
                        var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        playerBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                    }
                    else
                    {
                        playerBagSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Box:
                break;
        }
    }

    public void OpenBagUI()
    {
        _bagOpened = !_bagOpened;
        bagUI.SetActive(_bagOpened);
    }
    /// <summary>
    /// 更新Slot高亮显示
    /// </summary>
    /// <param name="index">序号</param>
    public void UpdateSlotHightlight(int index)
    {
        foreach (var slot in playerBagSlots)
        {
            if (slot.isSelected && slot.slotIndex == index)
            {
                slot.slotHightlight.gameObject.SetActive(true);
            }
            else
            {
                slot.isSelected = false;
                slot.slotHightlight.gameObject.SetActive(false);
            }
        }
    }
}
