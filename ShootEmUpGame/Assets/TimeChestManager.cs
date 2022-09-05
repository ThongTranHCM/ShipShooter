using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeChestManager : MonoBehaviour
{
    [System.Serializable]
    public class Mission{
        [SerializeField]
        string description;
        public string Description{
            get { return description; }
        }
        [SerializeField]
        int reward;
        public int Reward{
            get { return reward; }
        }
        [SerializeField]
        int interval;
        [SerializeField]
        int requirement;
        int curProgress;
        int startTime;
        public void AddProgress(){
            curProgress = Mathf.Min(curProgress + 1, requirement);
        }
        public void ResetProgress(){
            requirement = 0;
        }
        public void Complete(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            startTime = curTime + interval;
        }
        public bool IsActive(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            return curTime > startTime;
        }
        public (int,int) GetProgress(){
            return (curProgress, requirement);
        }
    }
    public class Data{
        public List<Mission> missionList;
        public int prevStartTime;
        public void InitData(){
            //missionList = GameInformation.Instance.dailyOfferList;
            int interval = GameInformation.Instance.timeChestInterval;
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            prevStartTime = curTime;
        }
    }
    private static TimeChestManager instance = null;
    public static TimeChestManager Instance{
        get { return instance; }
    }
    [SerializeField]
    private FillBarManager fillBarManager;
    [SerializeField]
    private FillBarTimeTextManager fillBarTimeTextManager;
    private Data data;

    TimeChestManager(){
        if(instance == null){
            instance = this;
        }
    }

    public void Save(){
        DataManager.Instance.timeChestManagerData = data;
        DataManager.Save();
    }

    void Start(){
        data = DataManager.Instance.timeChestManagerData;
        UpdateMissionPanel();
    }

    void FixedUpdate(){
        UpdateTimer();
    }

    private bool HasFinished(){
        int interval = GameInformation.Instance.timeChestInterval;
        if( GetCurTime() - data.prevStartTime > interval){
            data.prevStartTime = GetCurTime();
            Save();
            return true;
        }
        return false;
    }

    private int GetCurTime(){
        int interval = GameInformation.Instance.timeChestInterval;
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return curTime;
    }

    private void UpdateFillBar(){
        int interval = GameInformation.Instance.timeChestInterval;
        int curTime = GetCurTime();
        fillBarManager.SetRawValue(curTime - data.prevStartTime, interval);
        fillBarTimeTextManager.SetValue(interval);
    }

    private void UpdateTimer(){
        UpdateFillBar();
        if(HasFinished()){
            AutoClaimReward();
            UpdateMissionPanel();
        }
    }

    private void UpdateMissionPanel(){
        return;
    }

    private void PuchaseReward(){
        return;
    }

    private void AutoClaimReward(){
        RewardResourceManager.Instance.AddReward("gold", 1000);
        RewardResourceManager.Instance.AddReward("diamond", 1000);
        RewardResourceManager.Instance.GetBoxReward("regular_box");
        return;
    }
}
