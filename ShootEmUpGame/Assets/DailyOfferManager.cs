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
        string id;
        public string ID { get { return id;}}
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
        List<bool> history = new List<bool>();
        public (string, int) RewardTuple(){
            return (rewardId, rewardAmount);
        }
        public (string, int) CostTuple(){
            return (costId, costAmount);
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
        public void UpdateList(){
            List<Offer> tmp = GameInformation.Instance.dailyOfferList;
            for(int i = 0; i < tmp.Count; i++){
                if(offerList.Find( x => x.ID == tmp[i].ID) == null){
                    offerList.Add(tmp[i]);
                }
            }
            for(int i = 0; i < offerList.Count; i++){
                if(tmp.Find( x => x.ID == offerList[i].ID) == null){
                    offerList.Remove(offerList[i]);
                }
            }
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

    void Awake(){
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    public void UpdateContent(){
        if(data != null && DailyOfferContentManager.Instance != null){
            DailyOfferContentManager.Instance.UpdateOfferPanel(data.offerList, data.index);
            (string, int) cost = data.offerList[data.index].CostTuple();
            DailyOfferContentManager.Instance.UpdatePurchaseButton(cost.Item1, cost.Item2);
        } 
    }

    public void UpdateCounter(){
        if(data != null && DailyOfferContentManager.Instance != null){
            if(HasFinished()){
                RestartOffers();
                UpdateContent();
            }
            DailyOfferContentManager.Instance.SetCountDownText(GetCountDown());
        } 
    }

    public void InitContent(){
        if(data != null && DailyOfferContentManager.Instance != null){
            DailyOfferContentManager.Instance.SetCountDownText(GetCountDown());
            DailyOfferContentManager.Instance.UpdateOfferPanel(data.offerList, data.index);
            (string, int) cost = data.offerList[data.index].CostTuple();
            DailyOfferContentManager.Instance.UpdatePurchaseButton(cost.Item1, cost.Item2);
        } 
    }

    private void RestartOffers(){
        data.index = 0;
        DataManager.Save();
    }

    public void PurchaseReward(){
        if(data.index < data.offerList.Count){
            Debug.Log("Claim Reward");
            (string, int) reward = data.offerList[data.index].RewardTuple();
            (string, int) cost = data.offerList[data.index].CostTuple();
            List<(string, int)> rewardList = new List<(string, int)>();
            rewardList.Add(reward);
            if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                data.index += 1;
                UpdateContent();
                DataManager.Save();
            }
        } else {
            Debug.Log("You have collected all of the reward");
        }
    }

    public void InstancePurchaseReward(){
        Instance.PurchaseReward();
    }

    private int GetStartTime(){
        int interval = GameInformation.Instance.dailyOfferInterval;
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        int startTime = (int)(curTime/interval);
        startTime *= interval;
        return startTime;
    }

    private string GetCountDown(){
        int interval = GameInformation.Instance.dailyOfferInterval;
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
