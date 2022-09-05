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
    private List<Offer> offerList;
    private int prevStartTime;
    private int index;
    DailyOfferManager(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        InitData();
    }

    void FixedUpdate(){
        UpdateTimer();
    }

    public void UpdateTimer(){
        countDownText.text = GetCountDown();
        if(HasFinished()){
            prevStartTime = GetStartTime();
            InitData();
        }
    }

    private void InitData(){
        index = 0;
        offerList = GameInformation.Instance.dailyOfferList;
        int i = 0;
        foreach(Transform child in gameObject.transform){
            (string, int) tuple = offerList[i].RewardTuple();
            child.gameObject.GetComponent<ResourcePanelManager>().SetReward(tuple.Item1, tuple.Item2);
            i += 1;
        }
    }

    public void ClaimReward(){
        if(offerList[index] != null){
            (string, int) reward = offerList[index].RewardTuple();
            (string, int) cost = offerList[index].CostTuple();
            List<(string, int)> rewardList = new List<(string, int)>();
            rewardList.Add(reward);
            if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                index += 1;
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
        if(prevStartTime != GetStartTime()){
            prevStartTime = GetStartTime();
            return true;
        }
        return false;
    }
}
