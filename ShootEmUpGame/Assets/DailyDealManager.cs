using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DailyDealManager : MonoBehaviour
{
    [System.Serializable]
    public class Deal{
        [SerializeField]
        string id;
        public string ID { get { return id;}} // This is AddOnType
        private int level;
        public int Level{
            get { return level; }
        }
        private List<int> history = new List<int>();
        public void IncreaseLevel(){
            level = Mathf.Min(level + 1, GameInformation.Instance.dailyDealConversionList.Count - 1);
        }
        public void ResetLevel(){
            level = 0;
        }
        public int GetFragment(){
            return GameInformation.Instance.dailyDealConversionList[level].Fragment;
        }
        public int GetDiamond(){
            return GameInformation.Instance.dailyDealConversionList[level].Diamond;
        }
        public void UpdateHistory(){
            history.Add(level);
            if(history.Count > 100){
                history.RemoveAt(0);
            }
        }
        public float GetProb(int Index){
            if( history.Count == 0){
                return 0.5f;
            }
            return history.FindAll(x => x > Index).Count / history.Count;
        }
        public float GetBestDeal(){
            float sum = 0;
            for(int i = 0; i < GameInformation.Instance.dailyDealConversionList.Count; i++){
                float prob = GetProb(i);
                prob = prob > UnityEngine.Random.RandomRange(0.0f, 1.0f) ? 1 : 0;
                sum += GameInformation.Instance.dailyDealConversionList[i].Diamond * prob;
            }
            return sum + UnityEngine.Random.RandomRange(0.0f, 1.0f);
        }
    }
    [System.Serializable]
    public class Conversion{
        [SerializeField]
        private int fragment;
        public int Fragment{
            get { return fragment;}
        }
        [SerializeField]
        private int diamond;
        public int Diamond{
            get { return diamond;}
        }
    }
    public class Data{
        public int prevStartTime;
        public List<Deal> dealList;
        
        public void InitData(){
            int interval = GameInformation.Instance.dailyDealInterval;
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            int startTime = (int)(curTime/interval);
            startTime *= interval;
            dealList = GameInformation.Instance.dailyDealList;
            prevStartTime = startTime;
        }
        public void UpdateList(){
            List<Deal> tmp = GameInformation.Instance.dailyDealList;
            Debug.Log(dealList.Count);
            for(int i = 0; i < tmp.Count; i++){
                if(dealList.Find( x => x.ID == tmp[i].ID) == null){
                    dealList.Add(tmp[i]);
                }
            }
            for(int i = 0; i < dealList.Count; i++){
                if(tmp.Find( x => x.ID == dealList[i].ID) == null){
                    dealList.Remove(dealList[i]);
                }
            }
        }
    }

    private static DailyDealManager instance = null;
    public static DailyDealManager Instance{
        get { return instance; }
    }
    private Data data{
        get {
            if(DataManager.Instance != null){
                if(!isDataUpdated){
                    DataManager.Instance.dailyDealManagerData.UpdateList();
                    isDataUpdated = true;
                }
                return DataManager.Instance.dailyDealManagerData;
            }
            return null;
            } 
        set { 
            DataManager.Instance.dailyDealManagerData  = value;
        }
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

    public void UpdateCounter(){
        if(data != null && DailyDealContentManager.Instance != null){
            if(HasFinished() && DailyDealContentManager.Instance.IsInTab()){
                RestartDeals();
                UpdateContent();
            }
            DailyDealContentManager.Instance.SetTimeCounter(GetCountDown());
        }
    }

    public void UpdateContent(){
        if(data != null && DailyDealContentManager.Instance != null){
            DailyDealContentManager.Instance.UpdateDealPanel(data.dealList);
        }
    }

    public void InitContent(){
        if(data != null && DailyDealContentManager.Instance != null){
            DailyDealContentManager.Instance.SetTimeCounter(GetCountDown());
            DailyDealContentManager.Instance.UpdateDealPanel(data.dealList);
        }
    }

    private void RestartDeals(){
        int i = 0;
        foreach (Transform child in gameObject.transform){
            data.dealList[i].UpdateHistory();
            i += 1;
        }
        foreach(Deal deal in data.dealList){
            deal.ResetLevel();
        }
        SortDeal();
        data.prevStartTime = GetStartTime();
        DataManager.Save();
    }

    private int GetStartTime(){
        int interval = GameInformation.Instance.dailyDealInterval;
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        int startTime = (int)(curTime/interval);
        startTime *= interval;
        return startTime;
    }
    private string GetCountDown(){
        int interval = GameInformation.Instance.dailyDealInterval;
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        span = TimeSpan.FromSeconds(interval - (curTime - GetStartTime()) - 1);
        return string.Format("Reset in {0:0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
    }
    public bool HasFinished(){
        if( data != null){
            return data.prevStartTime != GetStartTime();
        } else {
            return false;
        }
    }
    private void SortDeal(){
        for(int i = data.dealList.Count - 1; i >= 0; i--){
            for(int j = 0; j < i; j++){
                if(data.dealList[j].GetBestDeal() < data.dealList[j + 1].GetBestDeal()){
                    Deal tmp = data.dealList[j];
                    data.dealList[j] = data.dealList[j + 1];
                    data.dealList[j + 1] = tmp;
                }
            }
        }
    }
    public void IncreaseLevel(Deal deal){
        deal.IncreaseLevel();
        UpdateContent();
    }
}
