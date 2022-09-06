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
        public string ID { get { return id;}}
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
        bool isActive = false;
        public bool IsActive{
            get { return isActive; }
        }
        public int Requirement{
            get { return requirement; }
        }
        int curProgress;
        public int CurProgress{
            get { return curProgress; }
        }
        int startTime;
        public void AddProgress(int Amount){
            if(isActive){
                curProgress = Mathf.Min(curProgress + Amount, requirement);
            }
        }
        public void Complete(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            startTime = curTime + interval;
            curProgress = 0;
            isActive = false;
        }
        public void UpdateActive(){
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            isActive = curTime > startTime;
        }
        public bool IsFinish(){
            return curProgress >= requirement;
        }
        public float GetPercentCompletion(){
            return (float)curProgress / requirement;
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
        public void UpdateList(){
            List<Mission> tmp = GameInformation.Instance.timeChestMissionList;
            for(int i = 0; i < tmp.Count; i++){
                if(missionList.Find( x => x.ID == tmp[i].ID) == null){
                    missionList.Add(tmp[i]);
                }
            }
            for(int i = 0; i < missionList.Count; i++){
                if(tmp.Find( x => x.ID == missionList[i].ID) == null){
                    missionList.Remove(missionList[i]);
                }
            }
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
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
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
            UpdateMissionActive();
            if(HasFinished()){
                ClaimReward();
            }
            TimeChestContentManager.Instance.SetFillBar(GetCurTime(), data.prevStartTime, GameInformation.Instance.timeChestInterval);
            if(prevData != data){   
                TimeChestContentManager.Instance.UpdatePurchaseButton();
                TimeChestContentManager.Instance.UpdateMissionPanel(data.missionList);
            }
            prevData = data;
        }
    }

    public void InitContent(){
        if(TimeChestContentManager.Instance != null && data != null){
            TimeChestContentManager.Instance.SetFillBar(GetCurTime(), data.prevStartTime, GameInformation.Instance.timeChestInterval);
            TimeChestContentManager.Instance.UpdatePurchaseButton();
            TimeChestContentManager.Instance.UpdateMissionPanel(data.missionList);
        }
    }

    private void UpdateMissionActive(){
        foreach(Mission mission in data.missionList){
            mission.UpdateActive();
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

    public void ProgressMission(string ID, int Amount = 1){
        foreach(Mission mission in data.missionList){
            Debug.Log(mission.ID);
            if(mission.ID == ID){
                mission.AddProgress(Amount);
            }
        }
    }

    public void CompleteMission(Mission TargetMission){
        TargetMission.Complete();
        UpdateMissionActive();
        data.prevStartTime -= TargetMission.Reward;
        DataManager.Save();
        TimeChestContentManager.Instance.UpdateFillBar(GetCurTime(), data.prevStartTime, GameInformation.Instance.timeChestInterval);
    }
}
