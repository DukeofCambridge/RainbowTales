using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //Item-in-scenery fade
    public const float FadeDuration = 0.35f;
    public const float TargetAlpha = 0.45f;

    //Time manager
    public const float SecondThreshold = 0.012f;
    public const int SecondHold = 59;
    public const int MinuteHold = 59;
    public const int HourHold = 23;
    public const int DayHold = 30;
    public const int SeasonHold = 3;
    
    //Transition
    public const float MaskFadeDuration = 0.8f;

    //割草数量限制
    public const int reapAmount = 2;

    //NPC网格移动
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;   //20*20 占 1 unit
    public const float animationBreakTime = 5f; //动画间隔时间
    public const int maxGridSize = 9999;

    //灯光
    public const float lightChangeDuration = 25f;
    //public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    //public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 playerStartPos = new Vector3(-1.7f, -5f, 0);
    public const int playerStartMoney = 100;
}
