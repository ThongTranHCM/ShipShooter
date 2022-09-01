using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyDealPanelManager : MonoBehaviour
{
    [SerializeField]
    private AddOnEquipData addOnEquipData;
    [SerializeField]
    private AddOnUIItem addOnUIItem;
    [SerializeField]
    private ResourceTextManager fragmentText;
    [SerializeField]
    private ResourceTextManager diamondText;

    public void SetDeal(DailyDealManager.Deal Deal){
        IAddOnData addOnData = addOnEquipData.GetAddOnData(Deal.Option.ID);
        addOnUIItem.Install(Deal.Option.ID, addOnData.GetSprite, (int)addOnData.GetLevel, 0, 100);
        fragmentText.SetResource("fragment");
        fragmentText.SetText(string.Format("+{0}",Deal.GetFragment().ToString()));
        diamondText.SetResource("diamond");
        diamondText.SetText(Deal.GetDiamondCost().ToString());
    }
}
