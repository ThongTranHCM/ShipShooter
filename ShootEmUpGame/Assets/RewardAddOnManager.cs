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
            if (data.GetLevel != 0){
                SoundManager.Instance.PlaySFX("open_box");
                FragmentRewardCanvasManager.Instance.Show(reward, 60);
            } else {
                SoundManager.Instance.PlaySFX("open_box");
                UnlockAddOnCanvasManager.Instance.Show(reward);
            }
        } else {
            FragmentRewardCanvasManager.Instance.Close();
            UnlockAddOnCanvasManager.Instance.Close();
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
}
