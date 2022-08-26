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
                GetPanel().transform.localScale = Vector3.one;
                PopUp(instance.resouceData.GetType(reward.Item1));
            } else {
                GetPanel().transform.localScale = Vector3.zero;
            }
        } else {
            instance.GetReward();
        }
    }

    private static void PopUp(ResourceData.Type Type){
        GameObject panel = GetPanel();
        GameObject content = panel.transform.GetChild(0).gameObject;
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.5f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);   
    }

    private static GameObject GetPanel(){
        return GameObject.FindWithTag("RewardPanelUI");
    }

    public void AddGold(int amount){
        if(instance == this){
            AddReward("Gold", amount);
        } else {
            instance.AddGold(amount);
        }
    }

    public void AddDiamond(int amount){
        if(instance == this){
            AddReward("Diamond", amount);
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
