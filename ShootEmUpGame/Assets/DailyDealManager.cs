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
        private int level;
        private int Level{
            get { return level; }
        }
        public void IncreaseLevel(){
            level = Mathf.Min(level + 1, GameInformation.Instance.dailyDealInterval);
        }
        public void ResetLevel(){
            level = 0;
        }
        public int GetFragment(){
            return GameInformation.Instance.dailyDealConversionList[level].Fragment;
        }
        public int GetDiamond(){
            return GameInformation.Instance.dailyDealConversionList[level].Diamond;
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
    [SerializeField]
    private TextMeshProUGUI countDownText;
    private int prevStartTime;
    private List<Deal> dealList;
    public void Start(){
        InitData();
    }
    public void FixedUpdate(){
        UpdateTimer();
    }
    public void UpdateTimer(){
        countDownText.text = GetCountDown();
        if(HasFinished()){
            Debug.Log("RESET");
            prevStartTime = GetStartTime();
            foreach(Deal deal in dealList){
                deal.ResetLevel();
            }
            InitData();
        }
    }
    private void InitData(){
        dealList = GameInformation.Instance.dailyDealList;
        int i = 0;
        foreach(Transform child in gameObject.transform){
            child.GetComponent<DailyDealPanelManager>().SetDeal(dealList[i]);
            i += 1;
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
