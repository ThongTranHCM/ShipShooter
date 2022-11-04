using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquippedAddOns : MonoBehaviour
{
    public List<Image> _listImgAddOns;
    public List<TMPro.TextMeshProUGUI> _listTxtLevelAddOns;
    [SerializeField]
    private string _gameMode;
    [SerializeField]
    private AddOnGroupLayout _groupLayout;

    public void Install(int index, Sprite spr)
    {
        if (index < 0 || index >= _listImgAddOns.Count) return;
        _listImgAddOns[index].sprite = spr;
    }
    public void Install(string gameMode)
    {
        _gameMode = gameMode;
        InstallEquippedAddOns();
    }

    public void InstallEquippedAddOns()
    {
        List<string> listStrAddOnEquiped = DataManager.Instance.addOnUserData.GetListAddOnEquiped(_gameMode);
        IAddOnData addOnData = null;
        for (int i = 0; i < _listImgAddOns.Count; i++)
        {
            addOnData = GameInformation.Instance.addOnEquipData.GetAddOnData(listStrAddOnEquiped[i]);
            if (addOnData != null)
            {
                _listImgAddOns[i].sprite = addOnData.GetSprite;
                _listTxtLevelAddOns[i].text = addOnData.GetLevel.ToString();
            }
            else
            {
                _listImgAddOns[i].sprite = null;
            }
        }
        _groupLayout.InstallMode(_gameMode);
    }
    
    public void OnEquipedClick(int index)
    {
        List<string> listStrAddOnEquiped = DataManager.Instance.addOnUserData.GetListAddOnEquiped(_gameMode);
        listStrAddOnEquiped[index] = "None";
        InstallEquippedAddOns();
    }
}
