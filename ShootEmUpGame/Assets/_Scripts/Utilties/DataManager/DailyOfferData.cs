using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyOfferData
{
    [System.Serializable]
    public class Reward{
        [SerializeField]
        string id;
        [SerializeField]
        int amount;
        public (string, int) ToTuple(){
            return (id, amount);
        }
    }
    private int startTime;
    public int StartTime{
        get { return startTime; }
    }
    private List<Reward> rewardList;
    public List<Reward> RewardList{
        get { return rewardList; }
    }
    const int interval = 24 * 3600;
    public void InitData(){
        rewardList = GameInformation.Instance.dailyOfferRewardList; 
        UpdateStartTime();
    }
    public bool HasFinished(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return (curTime - startTime) > interval;
    }
    public string GetCountDown(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        span = TimeSpan.FromSeconds(interval - (curTime - startTime));
        return string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }
    public void UpdateStartTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        startTime = (int)(span.TotalSeconds / interval);
        startTime *= interval;
        DataManager.Save();
    }
}
