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
        public bool UpdateActive(){
            if( isActive == false){
                TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
                int curTime = (int)span.TotalSeconds;
                isActive = curTime > startTime;
            }
            return isActive;
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
    private Data data{
        get {
            if(DataManager.Instance != null){
                if(!isDataUpdated){
                    DataManager.Instance.timeChestManagerData.UpdateList();
                    isDataUpdated = true;
                }
                return DataManager.Instance.timeChestManagerData;
            }
            return null; 
            } 
        set { DataManager.Instance.timeChestManagerData = value;}
    }
    private Data prevData = null;
    private bool isDataUpdated = false;

    void Awake(){
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    void FixedUpdate(){
        if ( data != null){
            UpdateMissionActive();
        }
    }

    private bool HasFinished(){
        if( data != null){
            int interval = GameInformation.Instance.timeChestInterval;
            if( GetCurTime() - data.prevStartTime > interval){
                return true;
            }
            return false;
        } else {
            return false;
        }
        
    }

    private int GetCurTime(){
        int interval = GameInformation.Instance.timeChestInterval;
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return curTime;
    }

    public string GetTimeCountDown(){
        if( data != null){
            int curTime = GetCurTime();
            int left = GameInformation.Instance.timeChestInterval - (curTime - data.prevStartTime);
            TimeSpan span = TimeSpan.FromSeconds(left);
            return string.Format("{0:0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
        } else {
            return "0:00:00";
        }
        
    }

    public void UpdateCounter(){
        if(TimeChestContentManager.Instance != null && data != null){
            UpdateMissionActive();
            if(HasFinished() && TimeChestContentManager.Instance.IsInTab()){
                ClaimReward();
            }
            int timeLeft = GetCurTime() - data.prevStartTime;
            int interval = GameInformation.Instance.timeChestInterval;
            TimeChestContentManager.Instance.SetFillBar(timeLeft, interval);
            TimeChestContentManager.Instance.UpdatePurchaseButton(GetDiamondCost());
        }
    }

    private int GetDiamondCost(){
        int timeLeft = GetCurTime() - data.prevStartTime;
        int interval = GameInformation.Instance.timeChestInterval;
        return  60 * (1 - timeLeft / interval);
    }

    public void UpdateContent(){
        if(TimeChestContentManager.Instance != null && data != null){
            TimeChestContentManager.Instance.UpdateMissionPanel(data.missionList);
        }
    }

    public void InitContent(){
        if(TimeChestContentManager.Instance != null && data != null){
            UpdateMissionActive();
            int timeLeft = GetCurTime() - data.prevStartTime;
            int interval = GameInformation.Instance.timeChestInterval;
            TimeChestContentManager.Instance.SetFillBar(timeLeft, interval);
            TimeChestContentManager.Instance.UpdatePurchaseButton(GetDiamondCost());
            TimeChestContentManager.Instance.UpdateMissionPanel(data.missionList);
        }
    }

    private void UpdateMissionActive(){
        if(data != null){
            bool update = false;
            foreach(Mission mission in data.missionList){
                if(mission.UpdateActive()){
                    update = true;
                }
            }
            if(update && TimeChestContentManager.Instance != null && data != null){
                UpdateContent();
            }
        }
    }

    private void PuchaseReward(){
        return;
    }

    private void ClaimReward(){
        data.prevStartTime = GetCurTime();
        DataManager.Save();
        RewardResourceManager.Instance.AddReward("gold", 1000);
        RewardResourceManager.Instance.AddReward("diamond", 1000);
        RewardResourceManager.Instance.GetBoxReward("regular_box");
        return;
    }

    public void ProgressMission(string ID, int Amount = 1){
        foreach(Mission mission in data.missionList){
            if(mission.ID == ID){
                mission.AddProgress(Amount);
            }
        }
    }

    public void CompleteMission(Mission TargetMission){
        if(TargetMission.IsActive){
            data.prevStartTime -= TargetMission.Reward;
        }
        TargetMission.Complete();
        UpdateMissionActive();
        DataManager.Save();
        int timeLeft = GetCurTime() - data.prevStartTime;
        int interval = GameInformation.Instance.timeChestInterval;
        TimeChestContentManager.Instance.UpdateFillBar(timeLeft, interval);
    }

    public bool CheckClaimNotification(){
        if(data != null){
            foreach(Mission mission in data.missionList){
                if(mission.IsFinish()){
                    return true;
                }
            }
        }
        return HasFinished();
    }
}
