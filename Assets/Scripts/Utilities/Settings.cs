using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //Item-in-scenery fade
    public const float FadeDuration = 0.35f;
    public const float TargetAlpha = 0.45f;

    //Time manager
    public const float SecondThreshold = 0.012f;  //to decide how much time(second) equals one second in game
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
    public const float GridCellSize = 1;
    public const float GridCellDiagonalSize = 1.41f;
    public const float PixelSize = 0.05f;   //20*20 占 1 unit
    public const float AnimationBreakTime = 15f; //动画间隔时间
    public const int MaxGridSize = 999;

    //灯光
    public const float lightChangeDuration = 30f;
    public static TimeSpan dawnTime = new TimeSpan(5, 0, 0);
    public static TimeSpan morningTime = new TimeSpan(7, 0, 0);
    public static TimeSpan duskTime = new TimeSpan(17, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 playerStartPos = new Vector3(-1.7f, -5f, 0);
    public const int playerStartMoney = 100;
}
