using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rainbow.Items
{
    public class ItemManager : MonoBehaviour
    {
        //a template item, it will be a real item only after the method 'Init(id)' is called
        public Item itemPrefab;
        private Transform _itemParent;
        //记录场景Item
        private Dictionary<string, List<SceneItem>> _sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnInstantiateItemInScene(int id, Vector3 position)
        {
            Item item = Instantiate(itemPrefab, position, quaternion.identity, _itemParent);
            item.itemID = id;
        }
        private void OnAfterSceneLoadedEvent()
        {
            _itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }
        /// <summary>
        /// 获得当前场景所有Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };

                currentSceneItems.Add(sceneItem);
            }
            //C# Dictionary will automatically create the key if there is not one
            _sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;

        }
        /// <summary>
        /// 刷新重建当前场景物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            //you must declare an 'out' variable but not assign value to it before passing it as a parameter
            if (_sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    //清场
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, _itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}

