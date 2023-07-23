using System.Collections;
using System.Collections.Generic;
using Rainbow.Inventory;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator[] animators;  //animators in children gameobject

    public SpriteRenderer holdItem;

    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypes;   //all possible animators

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>(); //key:part type; value:override animator

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnAfterSceneLoadedEvent()
    {
        
    }
    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        if (holdItem.enabled == false)
        {
            StartCoroutine(ShowItem(itemSprite));
        }
    }
    private IEnumerator ShowItem(Sprite itemSprite)
    {
        //Debug.Log("show item!");
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(0.5f);
        holdItem.enabled = false;
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW:不同的工具返回不同的动画在这里补全
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.Hoe => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            ItemType.Axe => PartType.Axe,
            ItemType.BreakTool => PartType.Break,
            ItemType.ReapTool => PartType.Reap,
            ItemType.Furniture => PartType.Carry,
            _ => PartType.None
        };

        if (isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            //Debug.Log("enable!!");
            if (currentType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else
            {
                holdItem.enabled = false;
            }
        }
        SwitchAnimator(currentType);
    }


    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}