using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeChestData : MonoBehaviour
{
    private int startTime;
    public int StartTime{
        get { return startTime; }
    }
    const int interval = 24 * 3600;
    public void InitData(){
        UpdateStartTime();
    }
    public string GetCountDown(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        span = TimeSpan.FromSeconds(interval - (curTime - startTime));
        return string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }
    public void UpdateStartTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        startTime = (int)(span.TotalSeconds);
        DataManager.Save();
    }
    public (string, int) GetReward(){
        /*
        int playerLevel = DataManager.Instance;
        return GetReward(playerLevel);
        */
        return ("gold",0);
    }
    public bool HasFinished(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return (curTime - startTime) > interval;
    }
}
