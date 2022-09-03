using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RewardResourceManager : MonoBehaviour
{
    private static RewardResourceManager instance = null;
    public static RewardResourceManager Instance{
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
        if(rewardQueue.Count > 0){
            (string, int) reward = rewardQueue.Dequeue();
            SoundManager.Instance.PlaySFX("open_box");
            RewardResourceCanvasManager.Instance.Show(reward.Item1, reward.Item2);
            IncreaseResource(reward.Item1, reward.Item2);
        } else {
            RewardResourceCanvasManager.Instance.Close();
        }
    }

    public void Purchase(string RequireResource, int RequireAmount, List<(string, int)> Rewards){
        int check = 0;
        switch( RequireResource ){
            case "gold":
                check = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                check = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return;
        }
        if(check >= RequireAmount){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                default:
                    return;
            }
            DataManager.Save();
            foreach((string,int) reward in Rewards){
                AddReward(reward.Item1, reward.Item2);
            }
            GetReward();
        } else {
            //Get More Resource;
        }
    }

    public void InstanceGetReward(){
        instance.GetReward();
    }

    public void GetBoxReward(string Id){
        if(rewardQueue.Count > 0){
            LTSeq seq = BoxRewardCanvasManager.Instance.Show(Id);
            seq.append(() => {GetReward();});
        }    
    }

    public void InstanceBoxReward(string Id){
        instance.GetBoxReward(Id);
    }

    public void BoxPurchase(string Box, string RequireResource, int RequireAmount, List<(string, int)> Rewards){
        int check = 0;
        switch( RequireResource ){
            case "gold":
                check = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                check = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return;
        }
        if(check >= RequireAmount){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                default:
                    return;
            }
            DataManager.Save();
            foreach((string,int) reward in Rewards){
                AddReward(reward.Item1, reward.Item2);
            }
            GetBoxReward(Box);
        } else {
            //Get More Resource;
        }
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
        if(scene.name == "MainMenu"){
            GetReward();
        }
    }

    private void IncreaseResource(string Id, int Amount){
        switch( Id ){
            case "gold":
                DataManager.Instance.playerData.Coin += Amount;
                break;
            case "diamond":
                DataManager.Instance.playerData.Coin += Amount;
                break;
            default:
                return;
        }
        DataManager.Save();
    }
}
