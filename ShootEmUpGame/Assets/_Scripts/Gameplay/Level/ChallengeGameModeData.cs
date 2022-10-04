using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChallengeModeData", menuName = "Shooter/ModeChallenge")]
public class ChallengeGameModeData : GameModeData
{
    [SerializeField]
    private int _shipIndex;
    [SerializeField]
    private RandomLevelDesignData[] _challengeLevels;


    public RandomLevelDesignData getLevelData(int index)
    {
        RandomLevelDesignData result = null;
        if (index >= 0 && index < _challengeLevels.Length)
        {
            result = _challengeLevels[index];
        }
        return result;
    }

    public override LevelDesignData GetLevelData()
    {
        int levelIndex = DataManager.Instance.selectedLevelIndex;
        LevelDesignData levelData = getLevelData(levelIndex);
        return levelData;
    }

    public override IEnumerator OnLoseGame()
    {
        return GamePlayManager.Instance.EndGame();
    }

    public override IEnumerator OnWinGame()
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

    public override IEnumerator StartGame()
    {
        throw new System.NotImplementedException();
    }
}