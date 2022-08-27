using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private static RewardManager instance = null;
    public static RewardManager Instance{
        get {return instance;}
    }
    [SerializeField]
    private ResourceData resouceData;
    private Queue<(string, int)> rewardQueue = new Queue<(string, int)>();

    public void Awake(){
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(string id, int amount){
        if(instance == this){
            rewardQueue.Enqueue((id, amount));
        } else {
            instance.AddReward(id, amount);
        }
        
    }

    public void GetReward(){
        if(instance == this){
            if(rewardQueue.Count > 0){
                (string, int) reward = rewardQueue.Dequeue();
                GetRewardCanvas().transform.localScale = Vector3.one;
                PopUp(reward.Item1, reward.Item2);
            } else {
                GetRewardCanvas().transform.localScale = Vector3.zero;
            }
        } else {
            instance.GetReward();
        }
    }

    private static void PopUp(string Type, int Amount){
        GameObject canvas = GetRewardCanvas();
        GameObject content = canvas.transform.GetChild(0).gameObject;
        GameObject resourcePanel = canvas.transform.Find("Content/ResourcePanel").gameObject;
        Debug.Log(Type);
        resourcePanel.GetComponent<ResourcePanelManager>().SetReward(Type, Amount);
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.5f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);   
    }

    private static GameObject GetRewardCanvas(){
        return GameObject.FindWithTag("RewardCanvas");
    }

    public void AddGold(int amount){
        if(instance == this){
            AddReward("gold", amount);
        } else {
            instance.AddGold(amount);
        }
    }

    public void AddDiamond(int amount){
        if(instance == this){
            AddReward("diamond", amount);
        } else {
            instance.AddDiamond(amount);
        }
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
