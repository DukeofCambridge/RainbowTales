using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Inventory;
using Rainbow.Items;
using UnityEngine;

namespace Rainbow.Player
{
    public class TriggerItemPickup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetails.canPickedup)
                {
                    InventoryManager.Instance.AddItem(item, true);
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}

