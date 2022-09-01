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
        } else {
            FragmentRewardCanvasManager.Instance.Close();
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

    public IAddOnData GetAddOnData(string Id){
        return addOnEquipData.GetAddOnData(Id);
    }

    private void IncreaseFragment(string Id, int Amount){
        DataManager.Instance.addOnUserData.GetAddOnInfo(addOnEquipData.GetType(Id)).CurrentFragment += Amount;
    }
}
