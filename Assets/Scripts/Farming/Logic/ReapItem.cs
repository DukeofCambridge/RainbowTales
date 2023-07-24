using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rainbow.Farming
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails _cropDetails;

        private Transform PlayerTransform => FindObjectOfType<global::Player>().transform;

        public void InitCropData(int ID)
        {
            _cropDetails = FarmingManager.Instance.GetCropDetails(ID);
        }

        /// <summary>
        /// 生成果实
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < _cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;

                if (_cropDetails.producedMinAmount[i] == _cropDetails.producedMaxAmount[i])
                {
                    //代表只生成指定数量的
                    amountToProduce = _cropDetails.producedMinAmount[i];
                }
                else    //物品随机数量
                {
                    amountToProduce = Random.Range(_cropDetails.producedMinAmount[i], _cropDetails.producedMaxAmount[i] + 1);
                }

                //执行生成指定数量的物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (_cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPlayerPosition(_cropDetails.producedItemID[i]);
                    }
                    else    //世界地图上生成物品
                    {
                        //判断应该生成的物品方向
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        //一定范围内的随机
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, _cropDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-_cropDetails.spawnRadius.y, _cropDetails.spawnRadius.y), 0);

                        EventHandler.CallInstantiateItemInScene(_cropDetails.producedItemID[i], spawnPos);
                    }
                }
            }

        }
    }
}