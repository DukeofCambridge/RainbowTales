using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map_SO/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName] public string sceneName;
    [Header("地图信息")] 
    public int gridWidth;
    public int gridHeight;
    [Header("左下角原点")]
    public int originX;
    public int originY;
    public List<TileProperty> tileProperties;
}
