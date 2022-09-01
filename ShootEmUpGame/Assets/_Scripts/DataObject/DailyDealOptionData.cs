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
        private float interval;
        private int lastTime;
        private bool isInit = false;
        const float gamma = 0.5f;

        public void Init(){
            if(!isInit){
                interval = 8 * 3600;
                lastTime = System.DateTime.Now.Second - 8 * 3600;
                isInit = true;
            }
        }

        public void Update(){
            Init();
            float estimateInterval = System.DateTime.Now.Second - lastTime;
            lastTime = System.DateTime.Now.Second;
            estimateInterval = estimateInterval * gamma + interval * (1 - gamma);
            interval = estimateInterval;
            Debug.Log(interval);
        }

        public float GetChosenProbability(){
            Init();
            float delay = System.DateTime.Now.Second - lastTime;
            return Mathf.Exp(-delay/interval);
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
        index = Mathf.Min(index, conversionList.Count - 1);
        return conversionList[index].Fragment;
    }
    public int GetDiamondCost(int index){
        index = Mathf.Min(index, conversionList.Count - 1);
        return conversionList[index].Diamond;
    }
}
