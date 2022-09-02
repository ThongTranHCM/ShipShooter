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
        private DailyDealData.Option option;
        public DailyDealData.Option Option{
            get{ return option; }
        }
        private DailyDealData dailyDealOptionData;

        public Deal(DailyDealData Data, DailyDealData.Option Option){
            dailyDealOptionData = Data;
            option = Option;
        }

        public void UpdateLevel(){
            option.IncreaseLevel();
        }

        public void UpdateProb(){
            option.Update();
        }

        public int GetFragment(){
            return dailyDealOptionData.GetFragment(option.GetLevel());
        }

        public int GetDiamondCost(){
            return dailyDealOptionData.GetDiamondCost(option.GetLevel());
        }

        public float BestDeal(){
            float max = 0;
            float diamondSum = 0;
            for(int i = 0; i < dailyDealOptionData.GetConversionListCount(); i++){
                diamondSum += dailyDealOptionData.GetDiamondCost(i);
                if(diamondSum * option.GetProbability(i) > max){
                    max = diamondSum * option.GetProbability(i);
                }
            }
            return max;
        }

        public void ResetLevel(){
            option.ResetLevel();
        }
    }
    [SerializeField]
    private int countDown;
    [SerializeField]
    private int interval;
    [SerializeField]
    private DailyDealData dailyDealOptionData;
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
        foreach(DailyDealData.Option option in dailyDealOptionData.OptionList){
            dealList.Add(new Deal(dailyDealOptionData, option));
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
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        countDown = cur_time % interval;
        TimeSpan span = TimeSpan.FromSeconds(countDown);
        countDownText.text = string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
        if(countDown == 0){
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
