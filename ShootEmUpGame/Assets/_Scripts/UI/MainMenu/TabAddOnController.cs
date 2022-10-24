using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabAddOnController : PanelController
{
    [SerializeField]
    private GameObject _addOnPopUpObj;
    public void ShowAddOnInfoPopup()
    {
        _addOnPopUpObj.SetActive(true);
    }
}
