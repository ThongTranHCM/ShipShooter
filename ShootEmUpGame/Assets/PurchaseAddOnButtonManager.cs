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

    public void PurchaseReward(){
        if(box == ""){
            RewardAddOnManager.Instance.Purchase(costId, costAmount, rewardId);
        } else {
            RewardAddOnManager.Instance.BoxPurchase(box, costId, costAmount, rewardId);
        }
        
    }
}
