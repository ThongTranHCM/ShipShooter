using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryModeData", menuName = "Shooter/ModeStory")]
public class StoryGameModeData : GameModeData
{
    [SerializeField]
    private RandomLevelDesignData[] _storyLevels;
    public override LevelDesignData GetLevelData()
    {
        int levelIndex = DataManager.Instance.selectedLevelIndex;
        return _storyLevels[levelIndex]; //Possible levelIndex - 1
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
        DataManager.Instance.LastLevelWin++;
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }

    public override IEnumerator StartGame()
    {
        throw new System.NotImplementedException();
    }
}
