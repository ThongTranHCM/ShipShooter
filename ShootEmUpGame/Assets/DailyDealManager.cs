using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DailyDealManager : MonoBehaviour
{
    private static DailyDealManager instance = null;
    public static DailyDealManager Instance{
        get { return instance; }
    }
    private List<DailyDealData.Deal> dealList;
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private GameObject addOnDealPanelPrefab;
    [SerializeField]
    private TextMeshProUGUI countDownText;
    private List<GameObject> addOnDealPanelList;
    DailyDealManager(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        ResetDeals();
    }

    void FixedUpdate(){
        UpdateTimer();
    }

    public void UpdateTimer(){
        countDownText.text = DataManager.Instance.dailyDealData.GetCountDown();
        if(DataManager.Instance.dailyDealData.HasFinished()){
            DataManager.Instance.dailyDealData.UpdateStartTime();
            foreach(DailyDealData.Deal deal in dealList){
                deal.UpdateProb();
                deal.ResetLevel();
            }
            ResetDeals();
        }
    }

    public void SortDeal(){
        for(int i = dealList.Count - 1; i >= 0; i--){
            for(int j = 0; j < i; j++){
                if(dealList[j].BestDeal() > dealList[j + 1].BestDeal()){
                    DailyDealData.Deal tmp = dealList[j];
                    dealList[j] = dealList[j + 1];
                    dealList[j + 1] = tmp;
                }
            }
        }
    }

    public void ResetDeals(){
        dealList = DataManager.Instance.dailyDealData.DealList;
        SortDeal();
        int i = 0;
        foreach(Transform child in gameObject.transform){
            child.GetComponent<DailyDealPanelManager>().SetDeal(dealList[i]);
            i += 1;
        }
    }
}
