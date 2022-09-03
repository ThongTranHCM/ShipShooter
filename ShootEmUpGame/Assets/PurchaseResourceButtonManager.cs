using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseResourceButtonManager : ResourceSinkButtonManager
{
    [SerializeField]
    private string rewardId;
    [SerializeField]
    private int rewardAmount;
    [SerializeField]
    private string box;

    public void SetReward(string Id, int Amount){
        rewardId = Id;
        rewardAmount = Amount;
    }

    public void SetBox(string Box){
        box = Box;
    }

    public void PurchaseReward(){
        if(box == ""){
            RewardResourceManager.Instance.Purchase(costId, costAmount, rewardId, rewardAmount);
        } else {
            RewardResourceManager.Instance.BoxPurchase(box, costId, costAmount, rewardId, rewardAmount);
        }
        
    }
}
