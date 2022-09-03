using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceSinkButtonManager : MonoBehaviour
{
    [SerializeField]
    protected string costId;
    [SerializeField]
    protected int costAmount;
    [SerializeField]
    protected ResourceTextManager costText;
    protected bool lastSufficentCheck = false;

    void Awake(){
        SetCost(costId, costAmount);
    }

    public void SetCost(string Id, int Amount){
        costId = Id;
        costAmount = Amount;
        costText.SetValue(Id, Amount.ToString());
    } 
}
