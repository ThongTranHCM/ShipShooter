using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyOfferData
{
    [System.Serializable]
    public class Reward{
        [SerializeField]
        string rewardId;
        [SerializeField]
        int rewardAmount;
        [SerializeField]
        string costId;
        [SerializeField]
        int costAmount;
        public (string, int) RewardTuple(){
            return (rewardId, rewardAmount);
        }
        public (string, int) CostTuple(){
            return (costId, costAmount);
        }
    }
    private int index;
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
    public void UpdateIndex(){
        index = Mathf.Min(index + 1, rewardList.Count - 1);
    }
    public void ResetIndex(){
        index = 0;
    }
    public Reward GetReward(){
        return rewardList[index];
    }
}
