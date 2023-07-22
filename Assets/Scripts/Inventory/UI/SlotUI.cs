using Rainbow.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour,IPointerClickHandler, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI amountText;
    public Image slotHightlight;
    [SerializeField] private Button button;
    [Header("格子类型")]
    public SlotType slotType;
    public bool isSelected;
    public int slotIndex;
    private InventoryUI InventoryUI => GetComponentInParent<InventoryUI>();

    //物品信息
    public ItemDetails itemDetails;
    public int itemAmount;


    private void Start()
    {
        isSelected = false;
        if (itemDetails==null)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// 更新格子UI和信息
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">持有数量</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        slotImage.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        slotImage.enabled = true;
        button.interactable = true;
    }

    /// <summary>
    /// 讲Slot更新为空
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;
            InventoryUI.UpdateSlotHightlight(-1);
            EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
        }

        itemDetails = null;
        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetails==null) return;
        isSelected = !isSelected;
        //slotHightlight.gameObject.SetActive(isSelected);
        InventoryUI.UpdateSlotHightlight(slotIndex);
        if (slotType == SlotType.Bag)
        {
            EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryUI.dragItem.transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails==null) return;
        InventoryUI.dragItem.sprite = slotImage.sprite;
        InventoryUI.dragItem.enabled = true;
        InventoryUI.dragItem.SetNativeSize();
        isSelected = true;
        InventoryUI.UpdateSlotHightlight(slotIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryUI.dragItem.enabled = false;
        // Debug.Log(eventData.pointerCurrentRaycast.gameObject);

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                return;

            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            int targetIndex = targetSlot.slotIndex;

            //在Player自身背包范围内交换
            if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
            {
                InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
            }

            //清空所有高亮显示
            InventoryUI.UpdateSlotHightlight(-1);
        }
        else    //测试扔在地上
        {
            if (itemDetails.canDropped)
            {
                //鼠标对应世界地图坐标
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

                EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            }
        }
    }
}