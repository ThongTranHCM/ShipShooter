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

    private Queue<(string, int)> rewardQueue = new Queue<(string, int)>();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            Debug.LogError("Instance  " + gameObject.name);
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    public void AddReward(string id, int amount)
    {
        if (instance == this)
        {
            rewardQueue.Enqueue((id, amount));
        }
        else
        {
            instance.AddReward(id, amount);
        }
    }

    public void AddBuyShip(int shipId)
    {
        if (instance == this)
        {
            //AddReward("gold", amount);
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
            //AddReward("diamond", amount);
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
            (string, int) reward = rewardQueue.Dequeue();
            SoundManager.Instance.PlaySFX("open_box");
            ShipResourceCanvasManager.Instance.SetContentUnlockShip("ba");
            ShipResourceCanvasManager.Instance.SetContentUnlockShip("ba");
            ShipResourceCanvasManager.Instance.Show(reward.Item1, reward.Item2);
            IncreaseResource(reward.Item1, reward.Item2);
        }
        else
        {
            ShipResourceCanvasManager.Instance.Close();
        }
    }

    public bool Purchase(string RequireResource, int RequireAmount, List<(string, int)> Rewards)
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
            foreach ((string, int) reward in Rewards)
            {
                Debug.LogError("Reward " + reward.Item1);
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


    private void IncreaseResource(string Id, int Amount)
    {
        string shipRewardPattern = "tShip([0-9])([A-Z][a-z]*)";
        Regex shipUpgradeRegex = new Regex("tShip([0-9])Upgrade");
        Regex shipBuyRegex = new Regex("tShip([0-9])Upgrade");
        try
        {
            foreach (Match match in Regex.Matches(Id, shipRewardPattern,
                                                  RegexOptions.None, System.TimeSpan.FromSeconds(1)))
                Debug.LogError("Found " + match.Value);

            DataManager.Save();
        }
        catch (RegexMatchTimeoutException)
        {
            // Do Nothing: Assume that timeout represents no match.
        }
    }
}