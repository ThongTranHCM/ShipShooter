using System.Linq;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameInformation : MonoBehaviour
{
    private static GameInformation _instance;
    public static GameInformation Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameInformation>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class MaterialStorageData
    {
        public Material matActionBtn;
        public Material matPositiveBtn;
        public Material matNegativeBtn;
        public Material matInvalidBtn;
        public Color colActionBtn;
        public Color colPositiveBtn;
        public Color colNegativeBtn;
        public Color colInvalidBtn;
    }
    public MaterialStorageData materialData;

    public AddOnEquipData addOnEquipData;
    public List<DOShipData> shipData;
    public int dailyDealInterval = 5;
    public int dailyOfferInterval = 5;
    public int timeChestInterval = 10;
    public List<DailyDealManager.Conversion> dailyDealConversionList;
    public List<DailyDealManager.Deal> dailyDealList;
    public List<DailyOfferManager.Offer> dailyOfferList;
    public List<TimeChestManager.Mission> timeChestMissionList;


    public DOShipData GetShipData(int index)
    {
        return shipData[index];
    }

    #region TEAM LEVEL HELPER
    public bool isLevelMaxed(int level)
    {
        return level >= maxLevel();
    }

    public int maxLevel()
    {
        return 10;
    }

    public int GetExpForLevel(int level)
    {
        if (level <= 0)
            return 0;
        if (level > maxLevel())
        {
            return 100;
        }
        return 100;
    }
    #endregion
}
