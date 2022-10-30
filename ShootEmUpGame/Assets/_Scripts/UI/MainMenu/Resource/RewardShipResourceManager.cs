using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;

public class RewardShipResourceManager : MonoBehaviour
{
    private static RewardShipResourceManager instance = null;
    public static RewardShipResourceManager Instance
    {
        get { return instance; }
    }

    private Queue<(int, bool)> rewardQueue = new Queue<(int, bool)>();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(int shipId, bool isUnlock)
    {
        if (instance == this)
        {
            rewardQueue.Enqueue((shipId, isUnlock));
        }
        else
        {
            instance.AddReward(shipId, isUnlock);
        }
    }

    public void AddBuyShip(int shipId)
    {
        if (instance == this)
        {
            AddReward(shipId, false);
        }
        else
        {
            instance.AddBuyShip(shipId);
        }
    }

    public void AddUnlockShip(int shipId)
    {
        if (instance == this)
        {
            AddReward(shipId, true);
        }
        else
        {
            instance.AddUnlockShip(shipId);
        }
    }

    public void GetReward()
    {
        if (rewardQueue.Count > 0)
        {
            (int, bool) reward = rewardQueue.Dequeue();
            int index = reward.Item1;
            DOShipData shipData = GameInformation.Instance.shipData[index];
            if (reward.Item2) //Unlock
            {
                ShipResourceCanvasManager.Instance.SetContentUnlockShip(shipData.spritePresentShip, shipData.shipName);
            }
            else
            {
                int level = DataManager.Instance.playerData.GetShipProgress(index).shipLevel;
                ShipResourceCanvasManager.Instance.SetContentShowUpdateShip(shipData.spritePresentShip, level, (int) shipData.GetPower(level), level + 1, (int)shipData.GetPower(level + 1));
            }
            SoundManager.Instance.PlaySFX("open_box");
            ShipResourceCanvasManager.Instance.Show();
            IncreaseResource(reward.Item1, reward.Item2);
        }
        else
        {
            ShipResourceCanvasManager.Instance.Close();
        }
    }

    public bool Purchase(string RequireResource, int RequireAmount, List<(int, bool)> Rewards)
    {
        if (DataManager.Instance.playerData.CheckPurchaseable(RequireResource, RequireAmount))
        {
            switch (RequireResource)
            {
                case "gold":
                    DataManager.Instance.playerData.Coin -= RequireAmount;
                    break;
                case "diamond":
                    DataManager.Instance.playerData.Diamond -= RequireAmount;
                    break;
                default:
                    return false;
            }
            DataManager.isChangeCurrency = true;
            DataManager.Save();
            foreach ((int, bool) reward in Rewards)
            {
                AddReward(reward.Item1, reward.Item2);
            }
            GetReward();
            return true;
        }
        else
        {
            BuyMoreResourceManager.Instance.OpenPopUp(RequireResource);
            return false;
        }
    }

    public void InstanceGetReward()
    {
        instance.GetReward();
    }

    private void IncreaseResource(int shipId, bool isUnlock)
    {
        PlayerData.ShipProgressData shipProgress = DataManager.Instance.playerData.GetShipProgress(shipId);
        if (isUnlock && shipProgress.shipLevel < 1)
        {
            shipProgress.shipLevel = Mathf.Max(shipProgress.shipLevel, 1);
        }
        else
        {
            shipProgress.shipLevel += 1;
        }
    }
}