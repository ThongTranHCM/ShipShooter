using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMissionController : PanelController
{
    [SerializeField]
    private PurchaseResourceButtonManager _btnTimeChest;
    public override void OnDataChange()
    {
        base.OnDataChange();
        if (DataManager.isChangeCurrency)
        {
            _btnTimeChest.Refresh();
        }
    }
}
