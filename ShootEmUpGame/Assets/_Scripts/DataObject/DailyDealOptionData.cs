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
        [SerializeField]
        private List<float> probList;
        private int level;
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
                probList.Add(0);
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
    public int GetConversionListCount(){
        return conversionList.Count;
    }
}
