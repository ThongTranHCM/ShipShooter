using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Challenge_X", menuName = "Data/ChallengeLevelDesignData", order = 1)]
public class ChallengeLevelDesignData : LevelDesignData
{
    [SerializeField]
    private int _shipIndex;
    [SerializeField]
    private RandomLevelDesignData[] _challengeLevels;

    public int ShipIndex { get { return _shipIndex; } }
    public RandomLevelDesignData getLevelData(int index)
    {
        RandomLevelDesignData result = null;
        if (index >= 0 && index < _challengeLevels.Length)
        {
            result = _challengeLevels[index];
        }
        return result;
    }
    public override IEnumerator EndGame()
    {
        if (TimeChestManager.Instance != null)
        {
            TimeChestManager.Instance.ProgressMission("clear_stage", 1);
        }
        int lastLevel = DataManager.Instance.GetLastChallengeIndex(_shipIndex);
        DataManager.Instance.SetLastChallengeLevelWin(_shipIndex, lastLevel + 1);
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }
}
