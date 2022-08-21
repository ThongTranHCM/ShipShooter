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
    public List<string> listAddOnEquiped;

    public void InitData()
    {
        listAddOnEquiped = new List<string>();
        listAddOnEquiped.Add("GunSplit");
        listAddOnEquiped.Add("None");
        listAddOnEquiped.Add("None");
        listAddOnEquiped.Add("None");
        listAddOnInfo = new List<AddOnInfo>();
        GetAddOnInfo(AddOnEquipData.AddOnType.GunEnergyOrb).CurrentLevel = 1;
    }

    public List<string> GetListAddOnEquiped()
    {
        return listAddOnEquiped;
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
}