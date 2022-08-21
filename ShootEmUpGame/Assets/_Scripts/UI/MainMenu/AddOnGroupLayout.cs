using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnGroupLayout : MonoBehaviour
{
    public AddOnEquipData addOnEquipData;
    public GameObject _prefab;
    public UIEquippedAddOns _uiEquipAddOn;
    [SerializeField]
    private bool _show0Level;
    [SerializeField]
    private bool _show0Fragment;
    [SerializeField]
    private bool _show0Level0Fragment;
    private List<AddOnUIItem> _listAddOnUI;

    public void OnEnable()
    {
        if (_listAddOnUI == null) _listAddOnUI = new List<AddOnUIItem>();
        Install(_show0Level, _show0Fragment, _show0Level0Fragment);
    }

    public void Install(bool show0Level, bool show0Fragment, bool show0Level0Fragment)
    {
        List<IAddOnData> listAddOnData = GameInformation.Instance.addOnEquipData.addOnDatas;
        List<AddOnEquipData.AddOnType> listQualifiedType = new List<AddOnEquipData.AddOnType>();
        string addOnName = "";
        AddOnUserData.AddOnInfo addOnInfo = null;

        for (int i = 0; i < listAddOnData.Count; i++)
        {
            addOnName = listAddOnData[i].GetAddOnType.ToString();
            addOnInfo = DataManager.Instance.addOnUserData.GetAddOnInfo(listAddOnData[i].GetAddOnType);
            if (!listAddOnData[i].IsUnlocked
                || (addOnInfo.CurrentLevel == 0 && !show0Level)
                || (addOnInfo.CurrentFragment == 0 && !show0Fragment)
                || (addOnInfo.CurrentFragment == 0 && addOnInfo.CurrentLevel == 0 && !show0Level0Fragment))
            {
                continue;
            }
            listQualifiedType.Add(listAddOnData[i].GetAddOnType);
        }
        UpdateListUISize(listQualifiedType.Count);
        for (int i = 0; i < listQualifiedType.Count; i++)
        {
            addOnName = listQualifiedType[i].ToString();
            addOnInfo = DataManager.Instance.addOnUserData.GetAddOnInfo(listQualifiedType[i]);
            AddOnUIItem _addOnUiItem = _listAddOnUI[i];
            int index = 0;
            while (index < addOnName.Length)
            {
                if ('A' <= addOnName[index] && addOnName[index] <= 'Z')
                {
                    addOnName = addOnName.Substring(0, index) + " " + addOnName.Substring(index, addOnName.Length - index);
                    index += 2;
                }
                index++;
            }
            int tmp = i;
            _addOnUiItem.Install(addOnName, listAddOnData[i].GetSprite, addOnInfo.CurrentLevel, addOnInfo.CurrentFragment, 100);
            _addOnUiItem.onBtnClick = () => OnAddOnItemClick(tmp);
        }
    }

    public void UpdateListUISize(int listSize)
    {
        int currentListSize = _listAddOnUI.Count;
        while (currentListSize > listSize)
        {
            currentListSize--;
            Destroy(_listAddOnUI[listSize].gameObject);
            _listAddOnUI.RemoveAt(listSize);
        }
        while (currentListSize < listSize)
        {
            currentListSize++;
            GameObject _addOnItem = Instantiate<GameObject>(_prefab, transform);
            AddOnUIItem _addOnUiItem = _addOnItem.GetComponent<AddOnUIItem>();
            _listAddOnUI.Add(_addOnUiItem);
        }
    }

    public void OnAddOnItemClick(int id)
    {
        IAddOnData addOnData = GameInformation.Instance.addOnEquipData.addOnDatas[id];
        List<string> listAddOn = DataManager.Instance.addOnUserData.listAddOnEquiped;
        int i = 0;
        for (; i < listAddOn.Count; i++)
        {
            if (listAddOn[i] == "None")
            {
                break;
            }
        }
        if (i < listAddOn.Count)
        {
            listAddOn[i] = addOnData.GetAddOnType.ToString();
            _uiEquipAddOn.InstallEquippedAddOns();
        }
    }
}
