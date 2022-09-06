using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeChestManager : MonoBehaviour
{
    [System.Serializable]
    public class Mission{
        [SerializeField]
        string id;
        public string ID{
            get { return id; }
        }
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
        public int Requirement{
            get { return requirement; }
        }
        int curProgress;
        public int CurProgress{
            get { return curProgress; }
        }
        int startTime;
        public void AddProgress(int Amount){
            curProgress = Mathf.Min(curProgress + Amount, requirement);
        }
        public void Complete(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            startTime = curTime + interval;
            curProgress = 0;
        }
        public bool IsActive(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            return curTime > startTime;
        }
        public bool IsFinish(){
            return curProgress >= requirement;
        }
    }
    public class Data{
        public List<Mission> missionList;
        public int prevStartTime;
        public void InitData(){
            missionList = GameInformation.Instance.timeChestMissionList;
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
    private TimeChestManager.Data data{
        get { return DataManager.Instance.timeChestManagerData;}
        set { DataManager.Instance.timeChestManagerData = value;}
    }
    private Data prevData = null;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    private bool HasFinished(){
        int interval = GameInformation.Instance.timeChestInterval;
        if( GetCurTime() - data.prevStartTime > interval){
            data.prevStartTime = GetCurTime();
            DataManager.Save();
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

    public void UpdateContent(){
        if(TimeChestContentManager.Instance != null && data != null){
            if(HasFinished()){
                ClaimReward();
            }
            TimeChestContentManager.Instance.UpdateFillBar(GetCurTime(), data.prevStartTime, GameInformation.Instance.timeChestInterval);
            TimeChestContentManager.Instance.UpdatePurchaseButton();
            if(prevData != data){   
                TimeChestContentManager.Instance.UpdateMissionPanel(data.missionList);
            }
        }
    }

    private void PuchaseReward(){
        return;
    }

    private void ClaimReward(){
        data.prevStartTime = GetCurTime();
        RewardResourceManager.Instance.AddReward("gold", 1000);
        RewardResourceManager.Instance.AddReward("diamond", 1000);
        RewardResourceManager.Instance.GetBoxReward("regular_box");
        return;
    }

    private void ProgressMission(string ID, int Amount = 1){
        foreach(Mission mission in data.missionList){
            if(mission.ID == ID){
                mission.AddProgress(Amount);
            }
        }
    }
}
