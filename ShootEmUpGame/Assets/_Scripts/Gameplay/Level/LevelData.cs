using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelData_0", menuName = "Shooter/LevelData")]
public class LevelData : ScriptableObject
{
    public List<EndlessLevelDesignData> LeagueLevelsData;
    public List<ChallengeLevelDesignData> ChallengeLevelsData;
}
