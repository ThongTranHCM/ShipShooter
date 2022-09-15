using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Challenge_X", menuName = "Data/ChallengeLevelDesignData", order = 1)]
public class ChallengeLevelDesignData : RandomLevelDesignData
{
    [SerializeField]
    private int _shipIndex;
    
    public int ShipIndex { get { return _shipIndex; } }
}
