using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public GameObject _rewardPanel;
    public ResourceData _RewardTypeSO;
    private Queue<GameReward> _rewardQueue;
    private class GameReward{
        private string _id;
        private ResourceData.Type _type;
        private int _amount;
        public GameReward(string ID, int Amount, ResourceData.Type Type){
            _id = ID;
            _amount = Amount;
            _type = Type;
        }
    }

    public void Awake(){
        _rewardQueue = new Queue<GameReward>();
    }

    public void AddQueue(string id, int amount){
        _rewardQueue.Enqueue(new GameReward(id, amount, _RewardTypeSO.GetType(id)));
        PopUp();
        _rewardPanel.SetActive(_rewardQueue.Count > 0);
    }

    public void PopQueue(){
        _rewardQueue.Dequeue();
        PopUp();
        _rewardPanel.SetActive(_rewardQueue.Count > 0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void PopUp(){
        GameObject content = _rewardPanel.transform.GetChild(0).gameObject;
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.5f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);
        
    }

    public void AddGold(int amount){
        AddQueue("Gold",amount);
    }
}
