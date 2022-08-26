using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private static RewardManager _instance = null;
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
        DontDestroyOnLoad(gameObject);
        if(_instance == null){
            _instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(string id, int amount){
        _rewardQueue.Enqueue(new GameReward(id, amount));
    }

    public void GetReward(){
        if(_rewardQueue.Count > 0){
            GameReward reward = _rewardQueue.Dequeue();
            GetPanel().transform.localScale = Vector3.one;
            PopUp(_resouceData.GetType(reward.Id));
        } else {
            GetPanel().transform.localScale = Vector3.zero;
        }
    }

    private void PopUp(ResourceData.Type Type){
        GameObject panel = GetPanel();
        GameObject content = panel.transform.GetChild(0).gameObject;
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.5f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);   
    }

    private GameObject GetPanel(){
        return GameObject.FindWithTag("RewardPanelUI");
    }

    public void AddGold(int amount){
        AddReward("Gold", amount);
    }

    public void AddDiamond(int amount){
        AddReward("Diamond", amount);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GetReward();
    }
}
