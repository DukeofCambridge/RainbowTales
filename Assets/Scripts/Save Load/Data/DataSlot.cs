using System.Collections;
using System.Collections.Generic;
using Rainbow.Transition;
using UnityEngine;

namespace Rainbow.Save
{
    public class DataSlot
    {
        /// <summary>
        /// 进度条，String是GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region 用来UI显示进度详情
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + "/" + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日/";
                }
                return string.Empty;
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Home" => "宅邸",
                        "01.Mondstadt" => "晨曦酒庄",
                        /*"02.Home" => "小木屋",
                        "03.Stall" => "市场",
                        "04.Path" => "小径",
                        "05.House01" => "Trace的家",*/
                        _ => string.Empty
                    };
                }
                return string.Empty;
            }
        }
        #endregion
    }
}
