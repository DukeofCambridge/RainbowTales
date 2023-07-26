using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC_Schedule_SO/ScheduleDataList")]
public class ScheduleDataList_SO : ScriptableObject
{
    public List<ScheduleDetails> scheduleList;
}
