using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseFragmentButtonManager : ResourceSinkButtonManager
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
            RewardFragmentManager.Instance.Purchase(costId, costAmount, rewardId, rewardAmount);
        } else {
            RewardFragmentManager.Instance.BoxPurchase(box, costId, costAmount, rewardId, rewardAmount);
        }
        
    }
}
