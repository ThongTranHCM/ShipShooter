using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseShipResourceButtonManager : ResourceSinkButtonManager
{
    [SerializeField]
    bool _isUnlock;
    [SerializeField]
    int _shipId;

    public void SetReward(int shipId, bool unlockShip)
    {
        _shipId = shipId;
        _isUnlock = unlockShip;
    }

    public bool CheckPurchaseReward()
    {
        Debug.LogError("CheckPurchaseReward" + _shipId);
        List<(int, bool)> tuples = new List<(int, bool)>();
        tuples.Add((_shipId, _isUnlock));
        return RewardShipResourceManager.Instance.Purchase(costId, costAmount, tuples);
    }

    public void PurchaseReward()
    {
        CheckPurchaseReward();
    }
}
