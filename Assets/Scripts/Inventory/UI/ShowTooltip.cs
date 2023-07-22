using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rainbow.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI _slotUI;
        private InventoryUI InventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            _slotUI = GetComponent<SlotUI>(); 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_slotUI.itemDetails!=null)
            {
                InventoryUI.itemTooltip.gameObject.SetActive(true);
                InventoryUI.itemTooltip.SetupTooltip(_slotUI.itemDetails, _slotUI.slotType);
                //set the generation position of the tooltip with regard to the cursor
                InventoryUI.itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                InventoryUI.itemTooltip.transform.position = transform.position + Vector3.up * 40;
            }
            else
            {
                InventoryUI.itemTooltip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryUI.itemTooltip.gameObject.SetActive(false);
        }
    }
}

