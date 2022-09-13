using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardFragmentManager : MonoBehaviour
{
    private static RewardFragmentManager instance = null;
    public static RewardFragmentManager Instance{
        get {return instance;}
    }
    [SerializeField]
    private AddOnEquipData addOnEquipData;
    private Queue<(string, int)> rewardQueue = new Queue<(string,int)>();
    public void Awake(){
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    
    public void AddReward(string ID, int Amount){
        if(instance == this){
            rewardQueue.Enqueue((ID, Amount));
        } else {
            instance.AddReward(ID, Amount);
        }
    }

    public void GetReward(){
        if(rewardQueue.Count > 0){
            (string,int) reward = rewardQueue.Dequeue();
            IAddOnData data = addOnEquipData.GetAddOnData(reward.Item1);
            SoundManager.Instance.PlaySFX("open_box");
            FragmentRewardCanvasManager.Instance.Show(reward.Item1, reward.Item2);
            IncreaseFragment(reward.Item1, reward.Item2);
            DataManager.isChangeResources = true;
        } else {
            FragmentRewardCanvasManager.Instance.Close();
        }
    }
    
    public void InstanceGetReward(){
        instance.GetReward();
    }

    public bool Purchase(string RequireResource, int RequireAmount, List<(string, int)> Rewards){
        int check = 0;
        switch( RequireResource ){
            case "gold":
                check = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                check = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return false;
        }
        if(check >= RequireAmount){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Diamond -= RequireAmount;
                    break;
                default:
                    return false;
            }
            DataManager.isChangeCurrency = true;
            DataManager.Save();
            foreach((string,int) reward in Rewards){
                AddReward(reward.Item1, reward.Item2);
            }
            GetReward();
            return true;
        } else {
            BuyMoreResourceManager.Instance.OpenPopUp(RequireResource);
            return false;
        }
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
        int check = 0;
        switch( RequireResource ){
            case "gold":
                check = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                check = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return false;
        }
        if(check >= RequireAmount){
            switch( RequireResource ){
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Diamond -= RequireAmount;
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

    public IAddOnData GetAddOnData(string Id){
        return addOnEquipData.GetAddOnData(Id);
    }

    private void IncreaseFragment(string Id, int Amount){
        DataManager.Instance.addOnUserData.GetAddOnInfo(addOnEquipData.GetType(Id)).CurrentFragment += Amount;
        DataManager.isChangeResources = true;
        DataManager.Save();
    }
}
