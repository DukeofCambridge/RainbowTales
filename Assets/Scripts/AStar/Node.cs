using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rainbow.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //网格坐标
        public int gCost = 0;   //距离Start格子的距离
        public int hCost = 0;   //距离Target格子的距离
        public int FCost => gCost + hCost;  //当前格子的值
        public bool isObstacle = false; //当前格子是否是障碍
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //比较选出最低的F值，返回-1，0，1
            int result = FCost.CompareTo(other.FCost);
            return result!=0?result:hCost.CompareTo(other.hCost);
        }
    }
}