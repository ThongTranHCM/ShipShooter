using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseResourceButtonManager : ResourceSinkButtonManager
{
    [System.Serializable]
    public class Reward{
        [SerializeField]
        string id;
        [SerializeField]
        int amount;
        public Reward(string Id, int Amount){
            id = Id;
            amount = Amount;
        }
        public (string, int) ToTuple(){
            return (id, amount);
        }
    }
    [SerializeField]
    private List<Reward> rewards;
    [SerializeField]
    private string box;

    public void SetReward(List<Reward> Rewards){
        rewards = Rewards;
    }

    public void SetBox(string Box){
        box = Box;
    }

    public void PurchaseReward(){
        List<(string,int)> tuples = new List<(string, int)>();
        foreach(Reward reward in rewards){
            tuples.Add(reward.ToTuple());
        }
        if(box == ""){
            RewardResourceManager.Instance.Purchase(costId, costAmount, tuples);
        } else {
            RewardResourceManager.Instance.BoxPurchase(box, costId, costAmount, tuples);
        }
        
    }
}
