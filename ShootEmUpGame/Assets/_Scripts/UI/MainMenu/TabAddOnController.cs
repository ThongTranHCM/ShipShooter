using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabAddOnController : PanelController
{
    [SerializeField]
    private DailyDealContentManager _dailyDealContent;
    [SerializeField]
    private AddOnGroupLayout _addOnGroupLayout;
    public override void OnDataChange()
    {
        base.OnDataChange();
        if (DataManager.isChangeProgress) {
            _addOnGroupLayout.Reinstall();
        }
        if (DataManager.isChangeProgress || DataManager.isChangeCurrency)
        {
            _dailyDealContent.UpdateContent();
            //You may need to update dailyDealContent. It is real time so no so far.
        }
    }
}
