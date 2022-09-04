using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DailyDealManager : MonoBehaviour
{
    [System.Serializable]
    public class Deal{
        [SerializeField]
        private string id;
        public string ID{
            get{ return id;}
        }
        private int level = 0;
        public string GetEventKey(){
            return string.Format("{0}_{1}_{2}","DailyDeal",id,level.ToString());
        }
        public void UpdateCounter(){
            for(int i = 0; i < level; i++){
                DataManager.Instance.eventCounter.LogKey(GetEventKey());
            }
            DataManager.Save();
        }
        public float GetProbability(int Index){
            return DataManager.Instance.eventCounter.Rate(GetEventKey(), 3, "Days");
        }
        public int GetLevel(){
            return level;
        }
        public void IncreaseLevel(){
            level = Mathf.Min(level + 1, GameInformation.Instance.dailyDealConversionList.Count - 1);
        }
        public void ResetLevel(){
            level = 0;
        }
        public int GetFragment(){
            return GameInformation.Instance.dailyDealConversionList[level].Fragment;
        }
        public int GetDiamondCost(){
            return GameInformation.Instance.dailyDealConversionList[level].Diamond;
        }
        public float BestDeal(){
            float max = 0;
            float diamondSum = 0;
            float prob = 0;
            for(int i = 0; i < GameInformation.Instance.dailyDealConversionList.Count; i++){
                prob = GetProbability(i);
                prob = UnityEngine.Random.Range(0.0f,1.0f) < prob ? 1 : 0;
                diamondSum += GameInformation.Instance.dailyDealConversionList[i].Diamond;
                if(diamondSum * prob > max){
                    max = diamondSum * prob;
                }
            }
            max += UnityEngine.Random.Range(0.0f,1.0f);
            return max;
        }
    }
    [System.Serializable]
    public class Conversion{
        [SerializeField]
        private int fragment;
        public int Fragment{
            get { return fragment;}
        }
        [SerializeField]
        private int diamond;
        public int Diamond{
            get { return diamond;}
        }
    }
    private static DailyDealManager instance = null;
    public static DailyDealManager Instance{
        get { return instance; }
    }
    private List<Deal> dealList;
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private GameObject addOnDealPanelPrefab;
    [SerializeField]
    private TextMeshProUGUI countDownText;
    private List<GameObject> addOnDealPanelList;
    private int prevStartTime = 0;
    private const int interval = 5;
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

    private int GetStartTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        int startTime = (int)(curTime/interval);
        startTime *= interval;
        return startTime;
    }

    private string GetCountDown(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        span = TimeSpan.FromSeconds(interval - (curTime - GetStartTime()) - 1);
        return string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }

    private bool HasFinished(){
        return prevStartTime != GetStartTime();
    }

    public void UpdateTimer(){
        countDownText.text = GetCountDown();
        if(HasFinished()){
            prevStartTime = GetStartTime();
            foreach(Deal deal in dealList){
                deal.UpdateCounter();
                deal.ResetLevel();
            }
            ResetDeals();
        }
    }

    public void SortDeal(){
        for(int i = dealList.Count - 1; i >= 0; i--){
            for(int j = 0; j < i; j++){
                if(dealList[j].BestDeal() > dealList[j + 1].BestDeal()){
                    Deal tmp = dealList[j];
                    dealList[j] = dealList[j + 1];
                    dealList[j + 1] = tmp;
                }
            }
        }
    }

    public void ResetDeals(){
        dealList = GameInformation.Instance.dailyDealDealList;
        foreach(Deal deal in dealList){
            deal.ResetLevel();
        }
        SortDeal();
        int i = 0;
        foreach(Transform child in gameObject.transform){
            child.GetComponent<DailyDealPanelManager>().SetDeal(dealList[i]);
            i += 1;
        }
    }
}
