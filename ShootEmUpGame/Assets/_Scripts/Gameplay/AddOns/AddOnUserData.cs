using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AddOnUserData
{
    [System.Serializable]
    public class AddOnInfo
    {
        [SerializeField]
        private AddOnEquipData.AddOnType _addOnType;
        public AddOnEquipData.AddOnType GetAddOnType
        {
            get { return _addOnType; }
        }
        [SerializeField]
        private int _currentLevel;
        public int CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }
        [SerializeField]
        private int _fragment;
        public int CurrentFragment
        {
            get { return _fragment; }
            set { _fragment = value; }
        }
        private int FragmentToLevelUp
        {
            get { return 100; }
        }
        public AddOnInfo()
        {
            _addOnType = AddOnEquipData.AddOnType.None;
            _currentLevel = 0;
        }

        public AddOnInfo(AddOnEquipData.AddOnType addOnType)
        {
            _addOnType = addOnType;
            _currentLevel = 0;
        }
    }
    private List<AddOnInfo> listAddOnInfo;
    public List<AddOnInfo> GetListAddOnInfo() { return listAddOnInfo; }
    private List<string> _listAddOnEquiped;
    private List<string> _listAddOnChallengeEquiped;
    private List<string> _listAddOnEndlessEquiped;

    public void InitData()
    {
        _listAddOnEquiped = new List<string>();
        _listAddOnEquiped.Add("None");
        _listAddOnEquiped.Add("None");
        _listAddOnEquiped.Add("None");
        _listAddOnEquiped.Add("None");
        _listAddOnChallengeEquiped = new List<string>();
        _listAddOnChallengeEquiped.Add("None");
        _listAddOnChallengeEquiped.Add("None");
        _listAddOnChallengeEquiped.Add("None");
        _listAddOnChallengeEquiped.Add("None");
        _listAddOnEndlessEquiped = new List<string>();
        _listAddOnEndlessEquiped.Add("None");
        _listAddOnEndlessEquiped.Add("None");
        _listAddOnEndlessEquiped.Add("None");
        _listAddOnEndlessEquiped.Add("None");
        listAddOnInfo = new List<AddOnInfo>();
    }

    public List<string> GetListAddOnEquiped(string mode)
    {
        List<string> listAddOn;
        switch (mode)
        {
            case Constants.MODE_Challenge:
                if (_listAddOnChallengeEquiped == null)
                {
                    _listAddOnChallengeEquiped = new List<string>();
                    _listAddOnChallengeEquiped.Add("None");
                    _listAddOnChallengeEquiped.Add("None");
                    _listAddOnChallengeEquiped.Add("None");
                    _listAddOnChallengeEquiped.Add("None");
                }
                listAddOn = _listAddOnChallengeEquiped;
                break;
            case Constants.MODE_Endless:
                if (_listAddOnEndlessEquiped == null)
                {
                    _listAddOnEndlessEquiped = new List<string>();
                    _listAddOnEndlessEquiped.Add("None");
                    _listAddOnEndlessEquiped.Add("None");
                    _listAddOnEndlessEquiped.Add("None");
                    _listAddOnEndlessEquiped.Add("None");
                }
                listAddOn = _listAddOnEndlessEquiped;
                break;
            case Constants.MODE_Story:
            default:
                if (_listAddOnEquiped == null)
                {
                    _listAddOnEquiped = new List<string>();
                    _listAddOnEquiped.Add("None");
                    _listAddOnEquiped.Add("None");
                    _listAddOnEquiped.Add("None");
                    _listAddOnEquiped.Add("None");
                }
                listAddOn = _listAddOnEquiped;
                break;
        }
        return listAddOn;
    }

    public AddOnInfo GetAddOnInfo(AddOnEquipData.AddOnType addOnType)
    {
        AddOnInfo result = null;
        for (int i = 0; i < listAddOnInfo.Count; i++)
        {
            if (listAddOnInfo[i].GetAddOnType.Equals(addOnType))
            {
                result = listAddOnInfo[i];
                break;
            }
        }
        if (result == null)
        {
            result = new AddOnInfo(addOnType);
            listAddOnInfo.Add(result);
        }
        return result;
    }

    public bool Upgrade(AddOnEquipData.AddOnType addOnType)
    {
        Debug.LogError("Upgrade");
        AddOnInfo result = null;
        for (int i = 0; i < listAddOnInfo.Count; i++)
        {
            if (listAddOnInfo[i].GetAddOnType.Equals(addOnType))
            {
                result = listAddOnInfo[i];
                break;
            }
        }
        if (result == null)
        {
            result = new AddOnInfo(addOnType);
            listAddOnInfo.Add(result);
        }
        int cost = 0;
        if (result.CurrentLevel < 1)
        {
            cost = GameInformation.Instance.addOnEquipData.GetUnlockCost();
        }
        else
        {
            cost = GameInformation.Instance.addOnEquipData.GetUpgradeCost(result.CurrentLevel);
        }
        if (result.CurrentFragment >= cost)
        {
            result.CurrentFragment -= cost;
            result.CurrentLevel++;
            DataManager.isChangeProgress = true;
            DataManager.isChangeResources = true;
            DataManager.Save();
            return true;
        }
        return false;
    }
}