using System.Collections;
using System.Collections.Generic;
using Rainbow.Map;
using UnityEngine;

namespace Rainbow.Farming
{
    public class LogGenerator : MonoBehaviour
    {
        private Grid _currentGrid;

        public int seedItemID;
        public int growthDays = 12;

        private void Awake()
        {
            _currentGrid = FindObjectOfType<Grid>();
        }

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = _currentGrid.WorldToCell(transform.position);

            if (seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                if (tile == null)
                {
                    tile = new TileDetails();
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }

                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}

