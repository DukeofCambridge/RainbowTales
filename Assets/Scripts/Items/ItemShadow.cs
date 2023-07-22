using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rainbow.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;
        private SpriteRenderer _itemShadow;

        private void Awake()
        {
            _itemShadow = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _itemShadow.sprite = itemSprite.sprite;
            _itemShadow.color = new Color(0, 0, 0, 0.6f);
        }
    }
}

