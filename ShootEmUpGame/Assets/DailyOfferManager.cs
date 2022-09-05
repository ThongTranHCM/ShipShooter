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
            offerList = GameInformation.Instance.dailyOfferList;
            prevStartTime = DailyOfferManager.Instance.GetStartTime();
            index = 0;
        }
        public void LoadData(){
            if(DataManager.Instance.dailyDealManagerData != null){
                offerList = DataManager.Instance.dailOfferManagerData.offerList;
                prevStartTime = DataManager.Instance.dailOfferManagerData.prevStartTime;
                index = DataManager.Instance.dailOfferManagerData.index;
            } else {
                InitData();
            }
        }
        public void SaveData(){
            DataManager.Instance.dailOfferManagerData = this;
            DataManager.Save();
        }
    }
    private static DailyOfferManager instance = null;
    public static DailyOfferManager Instance{
        get { return instance; }
    }
    [SerializeField]
    private GameObject offerListGameObject;
    [SerializeField]
    private TextMeshProUGUI countDownText;
    [SerializeField]
    private PurchaseResourceButtonManager purchaseButton;
    private Data data;
    DailyOfferManager(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        data = new Data();
        data.InitData();
        UpdateOfferPanel();
    }

    void FixedUpdate(){
        UpdateTimer();
    }

    public void UpdateTimer(){
        countDownText.text = GetCountDown();
        if(HasFinished()){
            data.prevStartTime = GetStartTime();
            UpdateOfferPanel();
        }
    }

    private void UpdateOfferPanel(){
        int i = 0;
        foreach(Transform child in gameObject.transform){
            (string, int) tuple = data.offerList[i].RewardTuple();
            child.gameObject.GetComponent<ResourcePanelManager>().SetReward(tuple.Item1, tuple.Item2);
            i += 1;
        }
    }

    public void ClaimReward(){
        if(data.offerList[data.index] != null){
            (string, int) reward = data.offerList[data.index].RewardTuple();
            (string, int) cost = data.offerList[data.index].CostTuple();
            List<(string, int)> rewardList = new List<(string, int)>();
            rewardList.Add(reward);
            if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                data.index += 1;
            }
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
            return true;
        }
        return false;
    }
}
