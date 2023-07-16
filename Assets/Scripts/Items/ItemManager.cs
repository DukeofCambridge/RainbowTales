using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Rainbow.Items
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;

        private void Start()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        }

        private void OnInstantiateItemInScene(int id, Vector3 position)
        {
            Item item = Instantiate(itemPrefab, position, quaternion.identity, itemParent);
            item.itemID = id;
        }
    }
}

