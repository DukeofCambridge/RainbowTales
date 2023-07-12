using System;
using Cinemachine;
using UnityEngine;

public class SwitchCameraBound : MonoBehaviour
{
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D polygonCollider2D =
            GameObject.FindGameObjectWithTag("BoundConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        //Call this if the bounding shape's points change at runtime
        cinemachineConfiner.InvalidatePathCache();
    }
}
