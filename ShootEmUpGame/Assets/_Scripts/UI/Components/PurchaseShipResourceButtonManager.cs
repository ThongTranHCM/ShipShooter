using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseShipResourceButtonManager : ResourceSinkButtonManager
{
    [SerializeField]
    int _amount;
    [SerializeField]
    string _shipId;

    public void SetReward(string shipId, int amount)
    {
        _shipId = shipId;
        _amount = amount;
    }

    public bool CheckPurchaseReward()
    {
        Debug.LogError("CheckPurchaseReward" + _shipId);
        List<(string, int)> tuples = new List<(string, int)>();
        tuples.Add((_shipId, _amount));
        return RewardShipResourceManager.Instance.Purchase(costId, costAmount, tuples);
    }

    public void PurchaseReward()
    {
        CheckPurchaseReward();
    }
}
