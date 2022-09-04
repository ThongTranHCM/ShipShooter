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
    private GameObject resourcePanelPrefab;
    [SerializeField]
    private TextMeshProUGUI countDownText;
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
    }

    public void ClaimReward(){
        (string, int) reward = DataManager.Instance.dailyOfferData.GetReward().RewardTuple();
        (string, int) cost = DataManager.Instance.dailyOfferData.GetReward().CostTuple();
        List<(string, int)> rewardList = new List<(string, int)>();
        rewardList.Add(reward);
        RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList);
        DataManager.Instance.dailyOfferData.UpdateIndex();
    }

    public void ClaimRewardAd(){
        (string, int) reward = DataManager.Instance.dailyOfferData.GetReward().RewardTuple();
        (string, int) cost = DataManager.Instance.dailyOfferData.GetReward().CostTuple();
        List<(string, int)> rewardList = new List<(string, int)>();
        rewardList.Add(reward);
        RewardResourceManager.Instance.Purchase(cost.Item1,cost.Item2,rewardList);
    }
}
