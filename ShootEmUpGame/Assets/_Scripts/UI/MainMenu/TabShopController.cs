using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabShopController : PanelController
{
    [SerializeField]
    private int[] _intDailyOfferPrice;
    [SerializeField]
    private PurchaseResourceButtonManager _btnClaimDailyOffer;
    [SerializeField]
    private Button _btnViewAdsDailyOffer;
    [SerializeField]
    private Button _btnGachaBox;
    [SerializeField]
    private Button _btnGachaPremiumBox;

    public override void OnDataChange()
    {
        base.OnDataChange();
        if (DataManager.isChangeCurrency)
        {
            //_btnClaimDailyOffer.set
        }
    }
}
