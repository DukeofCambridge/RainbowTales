using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Rainbow.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        [Header("种地地层与瓦片")] 
        public RuleTile waterTile;
        public RuleTile digTile;

        private Tilemap _waterTilemap;
        private Tilemap _digTilemap;

        private Season _currentSeason;
        
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>(); //key:scene name+coordinate; value:tile details 
        private Grid _currentGrid;

        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        /// <summary>
        /// refresh map every day
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            _currentSeason = season;

            foreach (var tile in tileDetailsDict)
            {
                //irrigation is valid for 1 day
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //holes dug are valid for 5 days
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                }
            }

            RefreshMap();
        }

        private void OnAfterSceneLoadedEvent()
        {
            _currentGrid = FindObjectOfType<Grid>();
            _digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            _waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
            RefreshMap();
        }
        /// <summary>
        /// 执行实际工具或物品功能
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <param name="itemDetails">物品信息</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = _currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                //Crop currentCrop = GetCropObject(mouseWorldPos);

                //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        //EventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.Hoe:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //音效
                        //EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //音效
                        //EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.BreakTool:
                        break;
                    /*case ItemType.ChopTool:
                        //执行收割方法
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        // Crop currentCrop = GetCropObject(mouseWorldPos);
                        //执行收割方法
                        currentCrop.ProcessToolAction(itemDetails, currentTile);
                        EventHandler.CallPlaySoundEvent(SoundName.Basket);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for (int i = 0; i < itemsInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticleEffectType.ReapableScenery, itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;

                    case ItemType.Furniture:
                        //在地图上生成物品 ItemManager
                        //移除当前物品（图纸）InventoryManager
                        //移除资源物品 InventoryManger
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                        break;*/
                }

                //UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// 根据地图信息生成字典
        /// </summary>
        /// <param name="mapData">地图信息</param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y
                };

                //字典的Key
                string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }


        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key">x+y+地图名字</param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.TryGetValue(key, out TileDetails tileDetails))
                return tileDetails;
            return null;
        }
        /// <summary>
        /// 根据鼠标网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (_digTilemap != null)
                _digTilemap.SetTile(pos, digTile);
        }

        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.gridY, 0);
            if (_waterTilemap != null)
                _waterTilemap.SetTile(pos, waterTile);
        }
        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        private void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
        }
        /// <summary>
        /// 刷新当前地图（本质上刷新的只有挖坑浇水这些动态信息）
        /// </summary>
        private void RefreshMap()
        {
            if (_digTilemap != null)
                _digTilemap.ClearAllTiles();
            if (_waterTilemap != null)
                _waterTilemap.ClearAllTiles();

            DisplayMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetDigGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    //TODO:种子
                }
            }
        }
    }
}