using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelData_0", menuName = "Shooter/LevelData")]
public class LevelData : ScriptableObject
{
    public List<ChallengeGameModeData> ChallengeLevelsData;

    [SerializeField] EndlessGameModeData _endlessGameMode;
    [SerializeField] StoryGameModeData _storyGameMode;


    public RandomLevelDesignData GetLevelDataFromChallengeShipAndIndex(int shipId, int challengeIndex)
    {
        return ChallengeLevelsData[shipId].getLevelData(challengeIndex);
    }
    public ChallengeGameModeData GetChallengeModeDataFromShip(int shipId)
    {
        return ChallengeLevelsData[shipId];
    }
    public EndlessGameModeData GetEndlessGameModeData()
    {
        return _endlessGameMode;
    }
    public StoryGameModeData GetStoryModeData()
    {
        return _storyGameMode;
    }
}
