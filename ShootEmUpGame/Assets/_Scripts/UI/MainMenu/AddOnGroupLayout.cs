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
    [SerializeField]
    private bool _allowEquip;
    [SerializeField]
    private bool _allowShowInfo;
    [SerializeField]
    private string _gameMode;
    private List<AddOnUIItem> _listAddOnUI;

    [SerializeField]
    private bool _updateParentSize;
    [SerializeField]
    private UnityEngine.UI.GridLayoutGroup _gridLayout;

    public void OnEnable()
    {
        if (_listAddOnUI == null) _listAddOnUI = new List<AddOnUIItem>();
        Install(_show0Level, _show0Fragment, _show0Level0Fragment);
    }

    public void InstallMode(string gameMode)
    {
        _gameMode = gameMode;
    }

    public void Reinstall()
    {
        Install(_show0Level, _show0Fragment, _show0Level0Fragment);
    }

    public void Install(bool show0Level, bool show0Fragment, bool show0Level0Fragment)
    {
        List<IAddOnData> listAddOnData = GameInformation.Instance.addOnEquipData.addOnDatas;
        List<AddOnEquipData.AddOnType> listQualifiedType = new List<AddOnEquipData.AddOnType>();
        string addOnName = "";
        AddOnUserData.AddOnInfo addOnInfo = null;
        int cost = 0;

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
            if (addOnInfo.CurrentLevel < 1)
            {
                cost = GameInformation.Instance.addOnEquipData.GetUnlockCost();
            }
            else
            {
                cost = GameInformation.Instance.addOnEquipData.GetUpgradeCost(addOnInfo.CurrentLevel);
            }
            _addOnUiItem.Install(addOnName, listAddOnData[i].GetSprite, addOnInfo.CurrentLevel, addOnInfo.CurrentFragment, cost);
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
            GameObject addOnItem = Instantiate<GameObject>(_prefab, transform);
            AddOnUIItem addOnUiItem = addOnItem.GetComponent<AddOnUIItem>();
            _listAddOnUI.Add(addOnUiItem);
        }
        if (_updateParentSize)
        {
            UpdateParentSize();
        }
    }

    public void OnAddOnItemClick(int id)
    {
        IAddOnData addOnData = GameInformation.Instance.addOnEquipData.addOnDatas[id];
        List<string> listAddOn = DataManager.Instance.addOnUserData.GetListAddOnEquiped(_gameMode);
        int emptySlot = -1;
        bool isEquipped = false;
        for (int i = 0; i < listAddOn.Count; i++)
        {
            if (listAddOn[i] == addOnData.GetAddOnType.ToString())
            {
                isEquipped = true;
            }
            if (listAddOn[i] == "None" && emptySlot < 0)
            {
                emptySlot = i;
            }
        }
        if (_allowShowInfo)
        {
            AddOnInfoCanvasManager.Instance.SetContentShow(addOnData);
            AddOnInfoCanvasManager.Instance.Show();
        }
        if (emptySlot >= 0)
        {
            if (_allowEquip && !isEquipped)
            {
                listAddOn[emptySlot] = addOnData.GetAddOnType.ToString();
                _uiEquipAddOn.InstallEquippedAddOns();
            }
        }
    }

    public void UpdateParentSize()
    {
        RectTransform rectTf = transform.parent as RectTransform;

        float width = rectTf.rect.width;
        int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - _gridLayout.padding.horizontal + _gridLayout.spacing.x + 0.001f) / (_gridLayout.cellSize.x + _gridLayout.spacing.x)));
        int minRows = Mathf.CeilToInt(_listAddOnUI.Count / (float)cellCountX);

        float minSpace = _gridLayout.padding.vertical + (_gridLayout.cellSize.y + _gridLayout.spacing.y) * minRows - _gridLayout.spacing.y;
        rectTf.SetHeight(minSpace);
    }
}
