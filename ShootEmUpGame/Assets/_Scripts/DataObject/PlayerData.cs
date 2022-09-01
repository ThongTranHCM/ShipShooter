using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * PlayerSetting : Class cấu hình hệ thống nhân vật bao gồm thú cưỡi (MountSetting) và thú cưng (Pet)
 * */
[System.Serializable]
public class PlayerData
{
    public bool isGuest
    {
        get
        {
            return !Social.localUser.authenticated;
        }
    }
    public string userId
    {
        get
        {
            if (string.IsNullOrEmpty(socialId))
                return SystemInfo.deviceUniqueIdentifier;
            else
                return socialId;
        }
    }

    public string socialId = string.Empty;
    public bool hasCreatedAccount
    {
        get
        {
            return !string.IsNullOrEmpty(name);
        }
    }
    public int Coin
    {
        get
        {
            return coin;
        }
        set
        {
            int oldValue = coin;
            coin = value;

            if (oldValue > value)
            {
            }
        }
    }
    public int coin;
    public int Diamond
    {
        get
        {
            return diamond;
        }
        set
        {
            int oldValue = diamond;
            diamond = value;

            if (oldValue > value)
            {
            }
        }
    }
    public int diamond;

    public string name = string.Empty;
    public string avatarUrl = string.Empty;
    public int level;
    public int changeNameCount;
    public int exp;

    public PlayerData()
    {
    }

    public void InitData()
    {
        level = 1;
        exp = 50;
        name = string.Empty;
    }

    public void AddExp(int exp)
    {
        this.exp += exp;
        while (true)
        {
            if (IsLevelMax(level))
            {
                this.exp = 0;
                break;
            }

            if (this.exp >= GetExpForLevel(level))
            {
                this.exp -= GetExpForLevel(level);
                level++;
            }
            else
            {
                break;
            }
        }
    }
    public bool IsLevelMax(int level)
    {
        return level >= 50;
    }

    public int GetExpForLevel(int level)
    {
        return 500;
    }

    [System.Serializable]
    public class ShipProgressData
    {
        public int shipId;
        public int shipLevel;
    }

    [SerializeField]
    private List<ShipProgressData> _listShipProgress;
    public List<ShipProgressData> ListShipProgress
    {
        get
        {
            if (_listShipProgress == null)
            {
                Debug.LogError("Create New Ship Progress");
                _listShipProgress = new List<ShipProgressData>();
                ShipProgressData startingShip = new ShipProgressData();
                startingShip.shipId = 0;
                startingShip.shipLevel = 1;
                _listShipProgress.Add(startingShip);
            }
            return _listShipProgress;
        }
    }
}

