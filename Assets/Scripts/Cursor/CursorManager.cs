using System.Collections;
using System.Collections.Generic;
using Rainbow.Farming;
using Rainbow.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite _currentSprite;   //存储当前鼠标图片
    private Image _cursorImage;
    private RectTransform _cursorCanvas;
    // only if two requirements are met can the cursor be used
    private bool _cursorEnable;
    private bool _cursorPosValid;
    //建造图标跟随
    //private Image buildImage;

    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;
    private ItemDetails currentItem;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;
    private Transform playerTransform => FindObjectOfType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }


    private void Start()
    {
        _cursorCanvas = GameObject.FindGameObjectWithTag("MaskCanvas").GetComponent<RectTransform>();
        _cursorImage = _cursorCanvas.GetChild(0).GetComponent<Image>();
        _currentSprite = normal;
        SetCursorImage(normal);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_cursorCanvas == null) return;

        _cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && _cursorEnable)
        {
            SetCursorImage(_currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            //buildImage.gameObject.SetActive(false);
        }
    }

    private void CheckPlayerInput()
    {
        //TODO: 按住鼠标连续操作
        if (Input.GetMouseButtonDown(0) && _cursorPosValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        _cursorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        _cursorImage.sprite = sprite;
        _cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 物品选择事件函数
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            _cursorEnable = false;
            _currentSprite = normal;
        }
        else    //物品被选中才切换图片
        {
            currentItem = itemDetails;
            
            //WORKFLOW:添加所有类型对应图片
            _currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.Axe => tool,
                ItemType.Hoe=> tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool=>tool,
                _ => normal,
            };
            _cursorEnable = true;
        }
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            -mainCamera.transform.position.z));
        // the unit length of GridPos is a tile
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);
        //Debug.Log(mouseGridPos);
        //建造图片跟随移动
        //buildImage.rectTransform.position = Input.mousePosition;

        //判断是否不在玩家使用范围内
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        //var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        if (currentTile != null)
        {
            CropDetails currentCrop = FarmingManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);

            //WORKFLOW:补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Hoe:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.BreakTool:
                case ItemType.ReapTool:
                case ItemType.Axe:
                    if (crop != null)
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else
                        SetCursorInValid();
                    break;
                /*case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(currentItem.itemID);

                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;*/
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    private void SetCursorValid()
    {
        _cursorPosValid = true;
        _cursorImage.color = new Color(1, 1, 1, 1);
    }
    private void SetCursorInValid()
    {
        _cursorPosValid = false;
        _cursorImage.color = new Color(1, 0, 0, 0.4f);
    }
}