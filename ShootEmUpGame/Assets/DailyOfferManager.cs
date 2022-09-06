using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DailyOfferManager : MonoBehaviour
{
    [System.Serializable]
    public class Offer{
        [SerializeField]
        string rewardId;
        [SerializeField]
        int rewardAmount;
        [SerializeField]
        string costId;
        [SerializeField]
        int costAmount;
        [SerializeField]
        float efficency;
        List<bool> history;
        public (string, int) RewardTuple(){
            return (rewardId, rewardAmount);
        }
        public (string, int) CostTuple(){
            return (costId, costAmount);
        }
        public void UpdateHistory(bool Claimed){
            history.Add(Claimed);
            if(history.Count > 100){
                history.RemoveAt(0);
            }
        }
        private float GetRate(){
            if( history.Count == 0){
                return 0.5f;
            }
            return history.FindAll(x => x == true).Count / history.Count;
        }
        public float GetValue(){
            return GetRate() * efficency;
        }
    }
    public class Data{
        public List<Offer> offerList;
        public int prevStartTime;
        public int index;
        public void InitData(){
            int interval = GameInformation.Instance.dailyDealInterval;
            TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            int curTime = (int)span.TotalSeconds;
            int startTime = (int)(curTime/interval);
            startTime *= interval;
            offerList = GameInformation.Instance.dailyOfferList;
            prevStartTime = startTime;
            index = 0;
        }
    }
    private static DailyOfferManager instance = null;
    public static DailyOfferManager Instance{
        get { return instance; }
    }
    private Data data{
        get { return DataManager.Instance.dailyOfferManagerData;} 
        set { DataManager.Instance.dailyOfferManagerData = value;} 
    }
    private Data prevData;

    void Awake(){
        if(instance == null){
            instance = this;
        }        
    }

    public void UpdateContent(){
        if(data != null && DailyOfferContentManager.Instance != null){
            if(HasFinished()){
                RestartOffers();
            }
            DailyOfferContentManager.Instance.SetCountDownText(GetCountDown());
            if(prevData != data){
                Debug.Log("UpdateOffer");
                DailyOfferContentManager.Instance.UpdateOfferPanel(data.offerList);
            }
            prevData = data;
        } 
    }

    private void RestartOffers(){
        for( int i = 0; i < data.offerList.Count; i++){
            if( i < data.index){
                data.offerList[i].UpdateHistory(true);
            } else {
                data.offerList[i].UpdateHistory(false);
            }
        }
        data.index = 0;
        DataManager.Save();
    }

    

    public void ClaimReward(){
        if(data.index < data.offerList.Count){
            if(data.offerList[data.index] != null){
                (string, int) reward = data.offerList[data.index].RewardTuple();
                (string, int) cost = data.offerList[data.index].CostTuple();
                List<(string, int)> rewardList = new List<(string, int)>();
                rewardList.Add(reward);
                if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                    data.index += 1;
                    DataManager.Save();
                }
            }   
        } else {
            Debug.Log("You have collected all of the reward");
        }
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
        return string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }

    private bool HasFinished(){
        if(data.prevStartTime != GetStartTime()){
            data.prevStartTime = GetStartTime();
            DataManager.Save();
            return true;
        }
        return false;
    }
}
