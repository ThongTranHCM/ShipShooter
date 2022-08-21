using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/EnemyData", order = 1)]
public class EnemyData: ScriptableObject
{
    public enum MovementBehavior {None, DivePlayer, ChasePlayer}
    public enum RotationBehavior {None, Glide, FocusPlayer}
    public float health = 1;
    public float attack = 1;
    public GameObject prefab;
    public GameObject gunObject;
    public DOGunData gunData;
    public MovementBehavior Move;
    public RotationBehavior Rotate;
    public List<PatternData> PatternDataList;
}
