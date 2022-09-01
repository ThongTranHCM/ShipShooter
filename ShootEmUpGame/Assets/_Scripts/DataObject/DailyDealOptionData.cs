using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "dailyDealOptionData", menuName = "Data/DailyDealOptionData")]
public class DailyDealOptionData : ScriptableObject
{
    [System.Serializable]
    public class Option{
        [SerializeField]
        private string id;
        public string ID{
            get{ return id;}
        }
        private List<float> intervals = new List<float>();
        private List<int> lastTimes = new List<int>();
        const int defaultInterval = 8 * 3600;
        const float gamma = 0.1f;
        
        public float GetInterval(int Index){
            for(int i = intervals.Count; i <= Index; i++){
                intervals.Add(defaultInterval);
            }
            return intervals[Index];
        }

        public void SetInterval(int Index, float Value){
            GetInterval(Index);
            intervals[Index] = Value;
        }

        public int GetLastTime(int Index){
            for(int i = intervals.Count; i <= Index; i++){
                lastTimes.Add(System.DateTime.Now.Second - defaultInterval);
            }
            return lastTimes[Index];
        }

        public void ResetLastTime(int Index){
            if(Index > lastTimes.Count){
                lastTimes[lastTimes.Count - 1] = System.DateTime.Now.Second;
            }
            lastTimes[Index] = System.DateTime.Now.Second;
        }

        public void UpdateInterval(int Index){
            float estimateInterval = System.DateTime.Now.Second - GetLastTime(Index);
            estimateInterval = estimateInterval * gamma + intervals[Index] * (1 - gamma);
            SetInterval(Index, estimateInterval);
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
    [SerializeField]
    private List<Option> optionList;
    public List<Option> OptionList{
        get {return optionList;}
    }
    public Option GetOption(string Id){
        foreach(Option option in optionList){
            if(option.ID == Id){
                return option;
            }
        }
        return null;
    }

    [SerializeField]
    private List<Conversion> conversionList;
    public int GetFragment(int index){
        return conversionList[index].Fragment;
    }
    public int GetDiamondCost(int index){
        return conversionList[index].Diamond;
    }
}
