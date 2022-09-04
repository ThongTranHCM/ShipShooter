using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyOfferManager : MonoBehaviour
{
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
    DailyOfferManager(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        ResetOffers();
    }

    void FixedUpdate(){
        UpdateTimer();
    }

    public void UpdateTimer(){
        countDownText.text = DataManager.Instance.dailyOfferData.GetCountDown();
        if(DataManager.Instance.dailyOfferData.HasFinished()){
            DataManager.Instance.dailyOfferData.UpdateStartTime();
            ResetOffers();
        }
    }

    public void ResetOffers(){
        DataManager.Instance.dailyOfferData.ResetIndex();
        List<DailyOfferData.Reward> rewards = DataManager.Instance.dailyOfferData.RewardList;
        int i = 0;
        foreach(Transform child in offerListGameObject.transform){
            (string, int) tuple = rewards[i].RewardTuple();
            child.gameObject.GetComponent<ResourcePanelManager>().SetReward(tuple.Item1, tuple.Item2);
            i = Mathf.Min(i + 1, rewards.Count - 1);
        }
        (string, int) cost = DataManager.Instance.dailyOfferData.GetReward().CostTuple();
        purchaseButton.SetCost(cost.Item1, cost.Item2);
    }

    public void ClaimReward(){
        if(DataManager.Instance.dailyOfferData.GetReward() != null){
            (string, int) reward = DataManager.Instance.dailyOfferData.GetReward().RewardTuple();
            (string, int) cost = DataManager.Instance.dailyOfferData.GetReward().CostTuple();
            List<(string, int)> rewardList = new List<(string, int)>();
            rewardList.Add(reward);
            if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                DataManager.Instance.dailyOfferData.UpdateIndex();
            }
        }
    }

    public void ClaimRewardAd(){
        if(DataManager.Instance.dailyOfferData.GetReward() != null){
            (string, int) reward = DataManager.Instance.dailyOfferData.GetReward().RewardTuple();
            (string, int) cost = DataManager.Instance.dailyOfferData.GetReward().CostTuple();
            List<(string, int)> rewardList = new List<(string, int)>();
            rewardList.Add(reward);
            if(RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList)){
                DataManager.Instance.dailyOfferData.UpdateIndex();
            }
        }
    }
}
