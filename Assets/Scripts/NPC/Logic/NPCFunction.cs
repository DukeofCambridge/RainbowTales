using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;
    private bool isOpen;

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            //关闭背包
            CloseShop();
        }
    }

    public void OpenShop()
    {
        isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    }

    public void CloseShop()
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    }
}
