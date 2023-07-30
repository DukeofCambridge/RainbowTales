using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public ItemTooltip itemTooltip;
    public Image dragItem;
    [Header("玩家背包UI")]
    [SerializeField] private GameObject bagUI;
    private bool _bagOpened;
    [SerializeField] private SlotUI[] playerBagSlots;
    
    [Header("商店仓库")]
    [SerializeField] private GameObject baseBag;
    public GameObject shopSlotPrefab;
    public GameObject boxSlotPrefab;
    [SerializeField] private List<SlotUI> baseBagSlots;
    public Image ownerImage;
    public Sprite[] owners=new Sprite[2];
    [Header("交易UI")]
    public TradeUI tradeUI;
    public TextMeshProUGUI playerMoneyText;
    private void Start()
    {
        for (int i = 0; i < playerBagSlots.Length; ++i)
        {
            playerBagSlots[i].slotIndex = i;
        }

        _bagOpened = bagUI.activeInHierarchy;
        playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
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
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
        EventHandler.ShowTradeUI += OnShowTradeUI;
    }

    private void OnDisable()
    {
        EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
        EventHandler.ShowTradeUI -= OnShowTradeUI;
    }

    /// <summary>
    /// 打开通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
    {
        GameObject prefab = slotType switch
        {
            SlotType.Shop => shopSlotPrefab,
            SlotType.Box => boxSlotPrefab,
            _ => null,
        };
        if (slotType == SlotType.Shop)
        {
            ownerImage.sprite = owners[0];
        }else if (slotType == SlotType.Box)
        {
            ownerImage.sprite = owners[1];
        }
        //生成背包UI
        baseBag.SetActive(true);

        baseBagSlots = new List<SlotUI>();

        for (int i = 0; i < bagData.itemsofInventory.Count; i++)
        {
            var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
            slot.slotIndex = i;
            baseBagSlots.Add(slot);
        }
        //if you are using the component "content size fitter", you'd better use the instruction below
        LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
        //open the player bag as well if shop ui is opened
        if (slotType == SlotType.Shop || slotType == SlotType.Box) 
        {
            bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
            bagUI.SetActive(true);
            _bagOpened = true;
        }
        //更新UI显示
        OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemsofInventory);
    }
    /// <summary>
    /// 关闭通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
    {
        baseBag.SetActive(false);
        itemTooltip.gameObject.SetActive(false);
        UpdateSlotHightlight(-1);

        foreach (var slot in baseBagSlots)
        {
            Destroy(slot.gameObject);
        }
        baseBagSlots.Clear();

        if (slotType == SlotType.Shop || slotType == SlotType.Box)
        {
            bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            bagUI.SetActive(false);
            _bagOpened = false;
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        UpdateSlotHightlight(-1);
    }
    private void OnShowTradeUI(ItemDetails item, bool isSell)
    {
        tradeUI.gameObject.SetActive(true);
        tradeUI.SetupTradeUI(item, isSell);
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
                for (int i = 0; i < baseBagSlots.Count; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                    }
                    else
                    {
                        baseBagSlots[i].UpdateEmptySlot();
                    }
                }
                break;
        }
        playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
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
