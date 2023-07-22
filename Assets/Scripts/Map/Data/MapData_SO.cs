using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map_SO/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName] public string sceneName;
    public List<TileProperty> tileProperties;
}
