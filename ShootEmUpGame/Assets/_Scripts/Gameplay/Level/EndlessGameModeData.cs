using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndlessModeData", menuName = "Shooter/ModeEndless")]

public class EndlessGameModeData : GameModeData
{
    [SerializeField]
    private List<EndlessLevelDesignData> EndlessLevelsData;

    public override LevelDesignData GetLevelData()
    {
        int rankId = 0;
        return EndlessLevelsData[rankId];
    }

    public override IEnumerator OnLoseGame()
    {
        if (TimeChestManager.Instance != null)
        {
            TimeChestManager.Instance.ProgressMission("clear_stage", 1);
        }
        //Endless Level. What important is progress rank base on trello https://trello.com/c/5nXoQ2Nv/4-th%C3%AAm-controller-endless
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }

    //You cannot win Endless
    public override IEnumerator OnWinGame()
    {
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }

    public override IEnumerator StartGame()
    {
        throw new System.NotImplementedException();
    }
}
