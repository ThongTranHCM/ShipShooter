using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMissionController : PanelController
{
    [SerializeField]
    private Button _btnTimeChest;

    [SerializeField]
    private Image _imgBtnTimeChest;
    public override void OnDataChange()
    {
        base.OnDataChange();
        if (DataManager.isChangeCurrency)
        {
            //_btnClaimDailyOffer.set
        }
    }
}
