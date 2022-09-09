using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseAddOnButtonManager : ResourceSinkButtonManager
{
    [SerializeField]
    private List<string> rewardId;
    [SerializeField]
    private string box;

    public void SetReward(List<string> Id){
        rewardId = Id;
    }

    public void SetBox(string Box){
        box = Box;
    }

    public bool CheckPurchaseReward(){
        if(box == ""){
            return RewardAddOnManager.Instance.Purchase(costId, costAmount, rewardId);
        } else {
            return RewardAddOnManager.Instance.BoxPurchase(box, costId, costAmount, rewardId);
        }
    }

    public void PurchaseReward(){
        CheckPurchaseReward();
    }
}
