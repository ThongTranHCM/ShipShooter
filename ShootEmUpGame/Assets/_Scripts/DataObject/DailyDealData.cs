using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyDealData
{
    [System.Serializable]
    public class Option{
        [SerializeField]
        private string id;
        public string ID{
            get{ return id;}
        }
        private List<float> probList = new List<float>();
        private int level = 0;
        const float gamma = 0.5f;
        public void Update(){
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
    private List<Conversion> conversionList;
    public List<Conversion> ConversionList{
        get { return conversionList; }
    }
    private List<Option> optionList;
    public List<Option> OptionList{
        get { return optionList; }
    }
    private int startTime;
    public int StartTime{
        get { return startTime; }
    }
    const int interval = 5;

    public void InitData(){
        conversionList = GameInformation.Instance.dailyDealConversionList;
        optionList = GameInformation.Instance.dailyDealOptionList;
        UpdateStartTime();
    }
    public int GetFragment(int index){
        index = Mathf.Min(index, conversionList.Count - 1);
        return conversionList[index].Fragment;
    }
    public int GetDiamondCost(int index){
        index = Mathf.Min(index, conversionList.Count - 1);
        return conversionList[index].Diamond;
    }
    public Option GetOption(string Id){
        foreach(Option option in optionList){
            if(option.ID == Id){
                return option;
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
