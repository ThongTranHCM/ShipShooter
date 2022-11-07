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
        int cost;
        if (addOnData.GetLevel < 1)
        {
            cost = GameInformation.Instance.addOnEquipData.GetUnlockCost();
        }
        else
        {
            cost = GameInformation.Instance.addOnEquipData.GetUpgradeCost(addOnData.GetLevel);
        }
        addOnUIItem.Install(deal.ID, addOnData.GetSprite, (int)addOnData.GetLevel, addOnData.GetFragment, cost);
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
            DataManager.isChangeCurrency = true;
            DataManager.isChangeProgress = true;
            DataManager.Save();
            SetDeal(deal);
        }
    }
}
