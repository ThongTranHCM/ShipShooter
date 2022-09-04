using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyDealData
{
    [System.Serializable]
    public class Deal{
        [SerializeField]
        private string id;
        public string ID{
            get{ return id;}
        }
        private List<float> probList = new List<float>();
        private int level = 0;
        const float gamma = 0.5f;
        public void UpdateProb(){
            for(int i = 0; i < probList.Count; i++){
                if(i < level){
                    probList[i] = probList[i] * gamma + (1 - gamma);
                } else {
                    probList[i] = probList[i] * gamma;
                }
            }
        }
        public float GetProbability(int Index){
            for(int i = probList.Count - 1; i < Index; i++){
                probList.Add(1);
            }
            return probList[Index];
        }
        public int GetLevel(){
            return level;
        }
        public void IncreaseLevel(){
            level += 1;
        }
        public void ResetLevel(){
            level = 0;
        }
        public int GetFragment(){
            return DataManager.Instance.dailyDealData.GetFragment(level);
        }
        public int GetDiamondCost(){
            return DataManager.Instance.dailyDealData.GetDiamondCost(level);
        }
        public float BestDeal(){
            float max = 0;
            float diamondSum = 0;
            float prob = 0;
            for(int i = 0; i < GameInformation.Instance.dailyDealConversionList.Count; i++){
                prob = GetProbability(i);
                prob = UnityEngine.Random.Range(0.0f,1.0f) < prob ? 1 : 0;
                diamondSum += DataManager.Instance.dailyDealData.GetDiamondCost(i);
                if(diamondSum * prob > max){
                    max = diamondSum * prob;
                }
            }
            //In case everything is 0, use this to randomize all of them;
            max += UnityEngine.Random.Range(0.0f,1.0f);
            return max;
        }
    }

    [System.Serializable]
    public class Conversion{
        [SerializeField]
        private int fragment;
        public int Fragment{
            get { return fragment; }
        }
        [SerializeField]
        private int diamond;
        public int Diamond{
            get { return diamond; }
        }
    }
    private List<Deal> dealList;
    public List<Deal> DealList{
        get { return dealList; }
    }
    private int startTime;
    public int StartTime{
        get { return startTime; }
    }
    const int interval = 5;

    public void InitData(){
        dealList = GameInformation.Instance.dailyDealDealList;
        UpdateStartTime();
    }
    public int GetFragment(int index){
        index = Mathf.Min(index, GameInformation.Instance.dailyDealConversionList.Count - 1);
        return GameInformation.Instance.dailyDealConversionList[index].Fragment;
    }
    public int GetDiamondCost(int index){
        index = Mathf.Min(index, GameInformation.Instance.dailyDealConversionList.Count - 1);
        return GameInformation.Instance.dailyDealConversionList[index].Diamond;
    }
    public DailyDealData.Deal GetDeal(string Id){
        foreach(DailyDealData.Deal deal in dealList){
            if(deal.ID == Id){
                return deal;
            }
        }
        return null; 
    }
    public bool HasFinished(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        return (curTime - startTime) > interval;
    }
    public string GetCountDown(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        int curTime = (int)span.TotalSeconds;
        span = TimeSpan.FromSeconds(interval - (curTime - startTime));
        return string.Format("Reset in {0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }
    public void UpdateStartTime(){
        TimeSpan span= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
        startTime = (int)(span.TotalSeconds / interval);
        startTime *= interval;
        DataManager.Save();
    }
}
