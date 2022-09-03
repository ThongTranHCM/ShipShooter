using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSinkButtonManager : MonoBehaviour
{
    [SerializeField]
    private string resourceId;
    [SerializeField]
    private int resourceCost;
    [SerializeField]
    private GameObject validButton;
    [SerializeField]
    private ResourceTextManager validCostText;
    [SerializeField]
    private GameObject invalidButton;
    [SerializeField]
    private ResourceTextManager invalidCostText;

    public void SetCost(string Id, int Cost){
        resourceId = Id;
        resourceCost = Cost;
        validCostText.SetValue(Id, Cost.ToString());
        invalidCostText.SetValue(Id, Cost.ToString());
    } 

    public void Update(){
        validCostText.SetValue(resourceId, resourceCost.ToString());
        invalidCostText.SetValue(resourceId, resourceCost.ToString());
        validButton.SetActive(IsSufficent());
        invalidButton.SetActive(!IsSufficent());
    }

    private bool IsSufficent(){
        int check = 0;
        switch(resourceId){
            case "gold":
                check = DataManager.Instance.playerData.Coin;
                break;
            case "diamond":
                check = DataManager.Instance.playerData.Diamond;
                break;
            default:
                return false;
        }
        return check > resourceCost;
    }
}
