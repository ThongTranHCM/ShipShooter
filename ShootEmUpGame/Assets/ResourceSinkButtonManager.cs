using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSinkButtonManager : MonoBehaviour
{
    [SerializeField]
    protected string resourceId;
    [SerializeField]
    protected int resourceCost;
    [SerializeField]
    protected GameObject validButton;
    [SerializeField]
    protected ResourceTextManager validCostText;
    [SerializeField]
    protected GameObject invalidButton;
    [SerializeField]
    protected ResourceTextManager invalidCostText;
    protected bool lastSufficentCheck = false;

    public void Update(){
        CheckUpdate();
    }

    public virtual void SetValue(string Id, int Cost){
        resourceId = Id;
        resourceCost = Cost;
        validCostText.SetValue(Id, Cost.ToString());
        invalidCostText.SetValue(Id, Cost.ToString());
    } 

    protected virtual bool IsSufficent(){
        int checkValue = 0;
        switch(resourceId){
            case "gold":
                checkValue = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                checkValue = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return false;
        }
        return checkValue > resourceCost;
    }

    public void CheckUpdate(){
        bool isSufficent = IsSufficent();
        if(lastSufficentCheck != isSufficent){
            validCostText.SetValue(resourceId, resourceCost.ToString());
            invalidCostText.SetValue(resourceId, resourceCost.ToString());
            validButton.SetActive(isSufficent);
            invalidButton.SetActive(!isSufficent);
            lastSufficentCheck = isSufficent;
        }
    }
}
