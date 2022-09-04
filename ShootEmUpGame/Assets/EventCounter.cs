using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventCounter : MonoBehaviour
{
    private const int limit = 3000;
    private Dictionary<string, List<int>> eventStampDictionary;
    public void InitData(){
        eventStampDictionary = new Dictionary<string, List<int>>();
    }

    private List<int> GetStamps(string Key){
        if (eventStampDictionary.ContainsKey(Key))
        {
            return eventStampDictionary[Key];
        }
        else
        {
            List<int> newStampList = new List<int>();
            eventStampDictionary.Add(Key, newStampList);
            return newStampList;
        }
    }

    public int Count(string Key, int Duration = -1){
        if( Duration == -1){
            return GetStamps(Key).Count;
        } else {
            int curTime = GetCurTime();
            int minTime = curTime - Duration;
            return GetStamps(Key).FindAll(x => x > minTime).Count;
        }
    }

    public float Rate(string Key, int Duration, string unit = "milliseconds"){
        if( Count(Key) == 0)
            return 0;
        int scale = 1;
        switch (unit) {
            case "Month":
                scale /= 30 / 7;
                goto case "Week";
            case "Week":
                scale /= 7;
                goto case "Day";
            case "Day":
                scale /= 24;
                goto case "Hour";
            case "Hour":
                scale /= 60;
                goto case "Minute";
            case "Minute":
                scale /= 60;
                goto case "Second";
            case "Second":
                scale /= 1000;
                goto case "MilliSecond";
            case "MilliSecond":
                scale = 1;
                goto default;
            default:
                break;
        }
        Duration = Mathf.FloorToInt(Duration * scale);
        return (float)Count(Key, Duration) / (float)Duration;
    }

    public void LogKey(string Key){
        int curTime = GetCurTime();
        GetStamps(Key).Add(curTime);
        if(GetStamps(Key).Count > limit){
            GetStamps(Key).RemoveAt(0);
        }
    }

    private int GetCurTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalMilliseconds;
        return curTime;
    }

    public void RemoveKey(string Key){
        if(eventStampDictionary.ContainsKey(Key)){
            eventStampDictionary.Remove(Key);
        }
    }

    public int GetLast(string Key){
        if( Count(Key) == 0)
            return -1;
        return GetStamps(Key)[GetStamps(Key).Count - 1];
    }

    public float OccurProbablity(string Key, int Duration, float Time, string Unit){
        if( Count(Key) == 0)
            return 0;
        float rate = Rate(Key, Duration, Unit);
        return 1 - Mathf.Exp(Time / rate);
    }

}
