using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSinkButtonManager : ResourceSinkButtonManager
{
    private string fragmentId;
    [SerializeField]
    private AddOnEquipData addOnEquipData;

    public override void SetValue(string Id, int Cost){
        fragmentId = Id;
        resourceId = "fragment";
        resourceCost = Cost;
        validCostText.SetValue(Id, Cost.ToString());
        invalidCostText.SetValue(Id, Cost.ToString());
    } 

    protected override bool IsSufficent(){
        AddOnUserData.AddOnInfo info = DataManager.Instance.addOnUserData.GetAddOnInfo(addOnEquipData.GetType(fragmentId));
        return info.CurrentFragment >= GetFragmentCost(info.CurrentLevel);
    }

    private int GetFragmentCost(int Level){
        return 1000;
    }
}
