using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndlessModeData", menuName = "Shooter/ModeEndless")]

public class EndlessGameModeData : GameModeData
{
    [System.Serializable]
    public class Reward
    {
        [SerializeField]
        public string id;
        [SerializeField]
        public int amount;
        public Reward(string Id, int Amount)
        {
            id = Id;
            amount = Amount;
        }
        public (string, int) ToTuple()
        {
            return (id, amount);
        }
    }
    [SerializeField]
    private List<EndlessLevelDesignData> EndlessLevelsData;
    [SerializeField]
    private int[] EndlessScoreThreshold;
    [SerializeField]
    private Reward[] EndlessRewards;

    public override LevelDesignData GetLevelData()
    {
        int rankId = 0;
        if (rankId < 0)
        {
            rankId = 0;
        }
        return EndlessLevelsData[rankId];
    }

    public override IEnumerator OnLoseGame()
    {
        GamePlayManager.Instance.RewardCollect();
        Debug.LogError("Lose");
        yield return null;
        GamePlayManager.Instance.QuitGame();
    }

    //You cannot win Endless
    public override IEnumerator OnWinGame()
    {
        if (TimeChestManager.Instance != null)
        {
            TimeChestManager.Instance.ProgressMission("clear_stage", 1);
        }
        Debug.LogError("Win");
        //Endless Level. What important is progress rank base on trello https://trello.com/c/5nXoQ2Nv/4-th%C3%AAm-controller-endless
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }

    public void UpdateEndlessRank()
    {
        int score = GamePlayManager.Instance.Collection.score;
        int rank = -1;
        int prevRank = DataManager.Instance.EndlessRank;
        Reward reward = null;
        if (DataManager.Instance.SetEndlessHighscore(score))
        {
            while (rank +1 < EndlessScoreThreshold.Length && score > EndlessScoreThreshold[rank + 1])
            {
                rank++;
                //Receive Reward
                if (rank > prevRank)
                {
                    reward = EndlessRewards[rank];
                    RewardResourceManager.Instance.AddReward(reward.id, reward.amount);
                }
            }
        }
        DataManager.Instance.EndlessRank = rank;
    }

    public override IEnumerator StartGame()
    {
        if (TimeChestManager.Instance != null)
        {
            TimeChestManager.Instance.ProgressMission("play_game", 1);
        }
        return GamePlayManager.Instance.StartGame();
    }
}
