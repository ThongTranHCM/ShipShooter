using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;

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
        if(instance == null)
        {
            Debug.LogError("Instance  " + gameObject.name);
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(string id, int amount){
        Debug.LogError("Instance  " + (instance == this) + " " + id + " " + amount);
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

    public bool Purchase(string RequireResource, int RequireAmount, List<(string, int)> Rewards){
        if(DataManager.Instance.playerData.CheckPurchaseable(RequireResource, RequireAmount)){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                default:
                    return false;
            }
            DataManager.isChangeCurrency = true;
            DataManager.Save();
            foreach((string,int) reward in Rewards)
            {
                Debug.LogError("Reward " + reward.Item1);
                AddReward(reward.Item1, reward.Item2);
            }
            GetReward();
            return true;
        } else {
            BuyMoreResourceManager.Instance.OpenPopUp(RequireResource);
            return false;
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

    public bool BoxPurchase(string Box, string RequireResource, int RequireAmount, List<(string, int)> Rewards){
        if(DataManager.Instance.playerData.CheckPurchaseable(RequireResource, RequireAmount)){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                default:
                    return false;
            }
            DataManager.isChangeCurrency = true;
            DataManager.Save();
            foreach((string,int) reward in Rewards){
                AddReward(reward.Item1, reward.Item2);
            }
            GetBoxReward(Box);
            return true;
        } else {
            BuyMoreResourceManager.Instance.OpenPopUp(RequireResource);
            return false;
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
        string shipRewardPattern = "tShip([0-9])([A-Z][a-z]*)";
        Regex shipRegex = new Regex("tShip([0-9])Upgrade");
        switch ( Id ){
            case "gold":
                DataManager.Instance.playerData.Coin += Amount;
                DataManager.isChangeCurrency = true;
                break;
            case "diamond":
                DataManager.Instance.playerData.Diamond += Amount;
                DataManager.isChangeCurrency = true;
                break;
            default:
                try
                {
                    foreach (Match match in Regex.Matches(Id, shipRewardPattern,
                                                          RegexOptions.None, System.TimeSpan.FromSeconds(1)))
                        Debug.LogError("Found " + match.Value);
                }
                catch (RegexMatchTimeoutException)
                {
                    // Do Nothing: Assume that timeout represents no match.
                }
                return;
        }
        DataManager.Save();
    }
}
