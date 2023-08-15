using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.Save;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.Spring;
    private int monthInSeason = 3;

    public bool gameClockPause=true;
    private float tikTime;
    //灯光时间差
    private float timeDifference;
    public string GUID => GetComponent<DataGUID>().guid;
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false;
    }

    private void Start()  //Start() is called after OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
        /*EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);*/
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;

            if (tikTime >= Settings.SecondThreshold)
            {
                tikTime -= Settings.SecondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; ++i)
            {
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.G))
        {
            gameDay += 1;
            EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
        }
    }
    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }
    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }
    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 28;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 3;
        gameYear = 845;
        gameSeason = Season.Spring;
    }
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.Pause;
    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.SecondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.MinuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.HourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.DayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                            gameMonth = 1;

                        monthInSeason--;
                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;

                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.SeasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;

                            if (gameYear > 9999)
                            {
                                gameYear = 2023;
                            }
                        }
                    }
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }

        // Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute);
    }
    /// <summary>
    /// 返回lightshift同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.dawnTime && GameTime < Settings.morningTime)
        {
            timeDifference = (float)(GameTime - Settings.dawnTime).TotalMinutes;
            return LightShift.Dawn;
        }
        if (GameTime >= Settings.morningTime && GameTime < Settings.duskTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if (GameTime >= Settings.duskTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.duskTime).TotalMinutes;
            return LightShift.Dusk;
        }
        if (GameTime < Settings.dawnTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            // Debug.Log(timeDifference);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear = saveData.timeDict["gameYear"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];
    }
}