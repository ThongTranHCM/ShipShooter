using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyDealPanelManager : MonoBehaviour
{
    [SerializeField]
    private AddOnEquipData addOnEquipData;
    [SerializeField]
    private AddOnUIItem addOnUIItem;
    [SerializeField]
    private ResourceTextManager fragmentText;
    [SerializeField]
    private Button button;
    private DailyDealManager.Deal deal;

    public void SetDeal(DailyDealManager.Deal Deal){
        deal = Deal;
        IAddOnData addOnData = addOnEquipData.GetAddOnData(deal.ID);
        addOnUIItem.Install(deal.ID, addOnData.GetSprite, (int)addOnData.GetLevel, addOnData.GetFragment, 1000);
        fragmentText.SetResource("fragment");
        fragmentText.SetText(string.Format("+{0}",deal.GetFragment().ToString()));
        List<PurchaseFragmentButtonManager.Reward> rewards = new List<PurchaseFragmentButtonManager.Reward>();
        rewards.Add(new PurchaseFragmentButtonManager.Reward(deal.ID, deal.GetFragment()));
        GetComponent<PurchaseFragmentButtonManager>().SetReward(rewards);
        GetComponent<PurchaseFragmentButtonManager>().SetCost("diamond", deal.GetDiamond());
    }

    public void PurchaseDeal(){
        if(GetComponent<PurchaseFragmentButtonManager>().CheckPurchaseReward()){
            DailyDealManager.Instance.IncreaseLevel(deal);
            DataManager.Save();
            SetDeal(deal);
        }
    }
}
