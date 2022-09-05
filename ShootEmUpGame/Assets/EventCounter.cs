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

    public int Count(string Key, int Duration = -1){
        if (eventStampDictionary.ContainsKey(Key))
        {
            if( Duration == -1){
                return eventStampDictionary[Key].Count;
            } else {
                int curTime = GetCurTime();
                int minTime = curTime - Duration;
                return eventStampDictionary[Key].FindAll(x => x >= minTime).Count;
            }
        } else {
            eventStampDictionary.Add(Key, new List<int>());
            return Count(Key, Duration);
        }
    }

    public float Rate(string Key, int Duration){
        if (eventStampDictionary.ContainsKey(Key))
        {
            if( Count(Key) == 0)
                return 0;
            Duration = Mathf.FloorToInt(Duration);
            return (float)Count(Key, Duration) / (float)Duration;
        } else {
            eventStampDictionary.Add(Key, new List<int>());
            return Count(Key, Duration);
        }
    }

    public void LogKey(string Key){
        if (eventStampDictionary.ContainsKey(Key))
        {
            int curTime = GetCurTime();
            eventStampDictionary[Key].Add(curTime);
            if(eventStampDictionary[Key].Count > limit){
                eventStampDictionary[Key].RemoveAt(0);
            }
        } else {
            eventStampDictionary.Add(Key, new List<int>());
            LogKey(Key);
        }
    }

    private int GetCurTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return curTime;
    }

    public void RemoveKey(string Key){
        if(eventStampDictionary.ContainsKey(Key)){
            eventStampDictionary.Remove(Key);
        }
    }

    public int GetLast(string Key){
        if (eventStampDictionary.ContainsKey(Key))
        {
            if(Count(Key) == 0)
                return -1;
            return eventStampDictionary[Key][eventStampDictionary[Key].Count - 1];
        } else {
            eventStampDictionary.Add(Key, new List<int>());
            return GetLast(Key);
        }
    }

    public float OccurProbablity(string Key, int Duration, float Time){
        if (eventStampDictionary.ContainsKey(Key))
        {
            if( Count(Key) == 0)
                return 0;
            float rate = Rate(Key, Duration);
            return 1 - Mathf.Exp(Time / rate);
        } else {
            eventStampDictionary.Add(Key, new List<int>());
            return GetLast(Key);
        }
    }

}
