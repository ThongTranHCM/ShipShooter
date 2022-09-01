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
    private ResourceTextManager diamondText;
    [SerializeField]
    private Button button;
    private DailyDealManager.Deal deal;

    public void SetDeal(DailyDealManager.Deal Deal){
        deal = Deal;
        IAddOnData addOnData = addOnEquipData.GetAddOnData(deal.Option.ID);
        addOnUIItem.Install(deal.Option.ID, addOnData.GetSprite, (int)addOnData.GetLevel, 0, 100);
        fragmentText.SetResource("fragment");
        fragmentText.SetText(string.Format("+{0}",deal.GetFragment().ToString()));
        diamondText.SetResource("diamond");
        diamondText.SetText(deal.GetDiamondCost().ToString());
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {SoundManager.Instance.PlaySFX("valid_button");});
        button.onClick.AddListener(() => {GetDeal();});
    }

    private void GetDeal(){
        RewardFragmentManager.Instance.AddReward(deal.Option.ID, deal.GetFragment());
        RewardFragmentManager.Instance.GetReward();
        deal.UpdateDeal();
        SetDeal(deal);
    }
}
