using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanelManager : MonoBehaviour
{
    [SerializeField]
    private string resourceID = "gold";
    [SerializeField]
    private int resourceAmount = 0;

    [SerializeField]
    private ResourceData resourceData;
    [SerializeField]
    private Image bigIcon;
    [SerializeField]
    private Image smallIcon;
    [SerializeField]
    private TextMeshProUGUI amountTxt;


    public void Start(){
        SetReward(resourceID, resourceAmount);
    }

    public void SetReward(string ID, int Amount){
        resourceID = ID;
        resourceAmount = Amount;
        UpdateUI(resourceID, resourceAmount);
    }

    private void UpdateUI(string ID, int Amount){
        ResourceData.Type type = resourceData.GetType(ID);
        bigIcon.sprite = type.bigIcon;
        bigIcon.rectTransform.localScale = type.scaleBig;
        bigIcon.preserveAspect = true;
        smallIcon.sprite = type.smallIcon;
        smallIcon.rectTransform.localScale = type.scaleSmall;
        smallIcon.preserveAspect = true;
        amountTxt.text = Amount.ToString();
    }
}
