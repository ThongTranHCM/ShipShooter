using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelData_0", menuName = "Shooter/LevelData")]
public class LevelData : ScriptableObject
{
    public List<EndlessLevelDesignData> EndlessLevelsData;
    public List<ChallengeLevelDesignData> ChallengeLevelsData;

    public EndlessLevelDesignData GetEndlessLevelDataFromRank(int rankIndex)
    {
        return EndlessLevelsData[rankIndex];
    }

    public RandomLevelDesignData GetLevelDataFromChallengeShipAndIndex(int shipId, int challengeIndex)
    {
        return ChallengeLevelsData[shipId].getLevelData(challengeIndex);
    }
}
