using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;
using WavePattern;

[CreateAssetMenu(fileName = "Data", menuName = "Data/PatternData", order = 1)]
public class PatternData : ScriptableObject
{
    public bool destroyOnComplete;
    public Difficulty difficulty = Difficulty.Easy;
    public float duration;
    public bool mirror;
    public Vector3 rotation;
    public List<SpawnInfo> enemyInstanceList = new List<SpawnInfo>();
}
