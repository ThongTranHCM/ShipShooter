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

    public void SetDeal(DailyDealManager.Deal Deal){
        IAddOnData addOnData = addOnEquipData.GetAddOnData(Deal.Option.ID);
        addOnUIItem.Install(Deal.Option.ID, addOnData.GetSprite, (int)addOnData.GetLevel, 0, 100);
        fragmentText.SetResource("fragment");
        fragmentText.SetText(string.Format("+{0}",Deal.GetFragment().ToString()));
        diamondText.SetResource("diamond");
        diamondText.SetText(Deal.GetDiamondCost().ToString());
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {SoundManager.Instance.PlaySFX("valid_button");});
        button.onClick.AddListener(() => {RewardAddOnManager.Instance.AddReward(Deal.Option.ID);});
        button.onClick.AddListener(() => {RewardAddOnManager.Instance.GetReward();});
    }
}
