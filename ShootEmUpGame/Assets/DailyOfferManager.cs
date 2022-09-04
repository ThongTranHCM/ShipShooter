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
        List<DailyOfferData.Reward> rewards = DataManager.Instance.dailyOfferData.RewardList;
        int i = 0;
        foreach(Transform child in offerListGameObject.transform){
            (string, int) tuple = rewards[i].ToTuple();
            child.gameObject.GetComponent<ResourcePanelManager>().SetReward(tuple.Item1, tuple.Item2);
            i = Mathf.Min(i + 1, rewards.Count - 1);
        }
    }
}