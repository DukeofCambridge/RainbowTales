using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.Spring;
    private int monthInSeason = 3;

    public bool gameClockPause=false;
    private float tikTime;

    private void Awake()
    {
        NewGameTime();
    }

    private void Start()  //Start() is called after OnEnable()
    {
        EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
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

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 3;
        gameYear = 845;
        gameSeason = Season.Spring;
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
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
        }

        // Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute);
    }
}