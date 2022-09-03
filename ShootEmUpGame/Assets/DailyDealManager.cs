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
    public class Deal{
        private string optionId;
        public string OptionId{
            get{ return optionId; }
        }

        public Deal(string Id){
            optionId = Id;
        }

        public void UpdateLevel(){
            DataManager.Instance.dailyDealData.GetOption(optionId).IncreaseLevel();
        }

        public void UpdateProb(){
            DataManager.Instance.dailyDealData.GetOption(optionId).Update();
        }

        public int GetFragment(){
            return DataManager.Instance.dailyDealData.GetFragment(DataManager.Instance.dailyDealData.GetOption(optionId).GetLevel());
        }

        public int GetDiamondCost(){
            return DataManager.Instance.dailyDealData.GetDiamondCost(DataManager.Instance.dailyDealData.GetOption(optionId).GetLevel());
        }

        public float BestDeal(){
            float max = 0;
            float diamondSum = 0;
            for(int i = 0; i < DataManager.Instance.dailyDealData.GetConversionListCount(); i++){
                diamondSum += DataManager.Instance.dailyDealData.GetDiamondCost(i);
                if(diamondSum * DataManager.Instance.dailyDealData.GetOption(optionId).GetProbability(i) > max){
                    max = diamondSum * DataManager.Instance.dailyDealData.GetOption(optionId).GetProbability(i);
                }
            }
            return max;
        }

        public void ResetLevel(){
            DataManager.Instance.dailyDealData.GetOption(optionId).ResetLevel();
        }
    }
    [SerializeField]
    private int countDown;
    [SerializeField]
    private int interval;
    private List<Deal> dealList;
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

    private List<Deal> GetAllDeals(){
        List<Deal> dealList = new List<Deal>();
        for(int i = 0 ; i < DataManager.Instance.dailyDealData.OptionList.Count ; i++ ){
            dealList.Add(new Deal(DataManager.Instance.dailyDealData.OptionList[i].ID));
        }
        return dealList;
    }

    private List<Deal> GetBestDeals(int Num){
        List<Deal> allList = GetAllDeals();
        List<Deal> bestList = new List<Deal>();
        for(int i = 0; i < Num; i++){
            Deal best = allList[0];
            foreach(Deal deal in allList){
                if(deal.BestDeal() > best.BestDeal()){
                    best = deal;
                }
            }
            bestList.Add(best);
            allList.Remove(best);
        }
        return bestList;
    }

    public void UpdateTimer(){
        countDownText.text = DataManager.Instance.dailyDealData.GetCountDown();
        if(DataManager.Instance.dailyDealData.HasFinished()){
            DataManager.Instance.dailyDealData.UpdateStartTime();
            foreach(Deal deal in dealList){
                deal.UpdateProb();
                deal.ResetLevel();
            }
            ResetDeals();
        }
    }

    public void ResetDeals(){
        dealList = GetBestDeals(3);
        addOnDealPanelList = new List<GameObject>();
        foreach(Transform child in gameObject.transform){
            GameObject.Destroy(child);
        }
        for(int i = 0; i < dealList.Count; i++){
            GameObject dealPanel = GameObject.Instantiate(addOnDealPanelPrefab,gameObject.transform.position, Quaternion.identity, gameObject.transform);
            dealPanel.GetComponent<DailyDealPanelManager>().SetDeal(dealList[i]);
            addOnDealPanelList.Add(dealPanel);
        }
    }
}
