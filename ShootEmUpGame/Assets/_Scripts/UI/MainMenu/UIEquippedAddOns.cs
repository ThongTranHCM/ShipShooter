using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquippedAddOns : MonoBehaviour
{
    public List<Image> _listImgAddOns;

    public void Install(int index, Sprite spr)
    {
        if (index < 0 || index >= _listImgAddOns.Count) return;
        _listImgAddOns[index].sprite = spr;
    }

    public void Start()
    {
        InstallEquippedAddOns();
    }

    public void InstallEquippedAddOns()
    {
        List<string> listStrAddOnEquiped = DataManager.Instance.addOnUserData.listAddOnEquiped;
        IAddOnData addOnData = null;
        for (int i = 0; i < 4; i++)
        {
            addOnData = GameInformation.Instance.addOnEquipData.GetAddOnData(listStrAddOnEquiped[i]);
            if (addOnData != null)
            {
                _listImgAddOns[i].sprite = addOnData.GetSprite;
            }
            else
            {
                _listImgAddOns[i].sprite = null;
            }
        }
    }
    
    public void OnEquipedClick(int index)
    {
        List<string> listStrAddOnEquiped = DataManager.Instance.addOnUserData.listAddOnEquiped;
        listStrAddOnEquiped[index] = "None";
        InstallEquippedAddOns();
    }
}
