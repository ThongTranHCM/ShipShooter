using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private static RewardManager _instance;
    public static RewardManager Instance
    {
        get { return _instance; }
    }
    public ResourceData _resouceData;
    private Queue<GameReward> _rewardQueue = new Queue<GameReward>();
    private class GameReward{
        private string _id;
        public string Id{
            get {return _id;}
        }
        private int _amount;
        public int Amount{
            get {return _amount;}
        }
        public GameReward(string ID, int Amount){
            _id = ID;
            _amount = Amount;
        }
    }

    public void Awake(){
        if(_instance == null){
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void AddReward(string id, int amount){
        _rewardQueue.Enqueue(new GameReward(id, amount));
    }

    public void GetReward(){
        if(_rewardQueue.Count > 0){
            Debug.Log("Has Reward");
            GameReward reward = _rewardQueue.Dequeue();
            PopUp(_resouceData.GetType(reward.Id));
            this.transform.localScale = Vector3.one;
        } else {
            Debug.Log("Empty");
            this.transform.localScale = Vector3.zero;
        }
    }

    private void PopUp(ResourceData.Type Type){
        GameObject content = this.transform.GetChild(0).gameObject;
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.5f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);   
    }

    public void AddGold(int amount){
        Debug.Log("Add Gold");
        AddReward("Gold", amount);
    }

    public void AddDiamond(int amount){
        Debug.Log("Add Diamond");
        AddReward("Diamond", amount);
    }
}
