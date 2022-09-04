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
        IAddOnData addOnData = addOnEquipData.GetAddOnData(deal.ID);
        addOnUIItem.Install(deal.ID, addOnData.GetSprite, (int)addOnData.GetLevel, addOnData.GetFragment, 1000);
        fragmentText.SetResource("fragment");
        fragmentText.SetText(string.Format("+{0}",deal.GetFragment().ToString()));
        diamondText.SetResource("diamond");
        diamondText.SetText(deal.GetDiamondCost().ToString());
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {SoundManager.Instance.PlaySFX("valid_button");});
        button.onClick.AddListener(() => {GetDeal();});
        button.onClick.AddListener(() => {DataManager.Save();});
    }

    private void GetDeal(){
        RewardFragmentManager.Instance.AddReward(deal.ID, deal.GetFragment());
        RewardFragmentManager.Instance.GetReward();
        deal.IncreaseLevel();
        SetDeal(deal);
    }
}
