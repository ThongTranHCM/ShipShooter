using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardAddOnManager : MonoBehaviour
{
    private static RewardAddOnManager instance = null;
    public static RewardAddOnManager Instance{
        get {return instance;}
    }
    [SerializeField]
    private AddOnEquipData addOnEquipData;
    private Queue<string> rewardQueue = new Queue<string>();
    public void Awake(){
        DontDestroyOnLoad(gameObject);
        if(instance == null){
            instance = this;
        } else {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(string id){
        if(instance == this){
            rewardQueue.Enqueue(id);
        } else {
            instance.AddReward(id);
        }
    }

    public void GetReward(){
        if(rewardQueue.Count > 0){
            string reward = rewardQueue.Dequeue();
            IAddOnData data = addOnEquipData.GetAddOnData(reward);
            SoundManager.Instance.PlaySFX("open_box");
            UnlockAddOnCanvasManager.Instance.Show(reward);
            DataManager.isChangeResources = true;
        } else {
            UnlockAddOnCanvasManager.Instance.Close();
        }
    }

    public void InstanceGetReward(){
        instance.GetReward();
    }

    public bool Purchase(string RequireResource, int RequireAmount, List<string> AddOns){
        if(DataManager.Instance.playerData.CheckPurchaseable(RequireResource, RequireAmount)){
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
            foreach(string addOn in AddOns){
                AddReward(addOn);
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

    public bool BoxPurchase(string Box, string RequireResource, int RequireAmount, List<string> AddOns){
        if(DataManager.Instance.playerData.CheckPurchaseable(RequireResource, RequireAmount)){
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
            foreach(string addOn in AddOns){
                AddReward(addOn);
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
}
