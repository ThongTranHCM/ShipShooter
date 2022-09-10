using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "shipProressData", menuName = "Shooter/ShipProgressData")]
public class DOShipProgressData : ScriptableObject
{
    [SerializeField]
    private int _shipCost;
    public int ShipCost { get { return _shipCost; } }
    [SerializeField]
    private int _maxLevel;
    public int MaxLevel { get { return _maxLevel; } }

    [SerializeField]
    private List<int> _shipPowers;
    public int GetPower(int level)
    {
        if (level < 0 || level > _maxLevel)
        {
            return -1;
        }
        return _shipPowers[level];
    }
    [SerializeField]
    private List<int> _shipUpgradeCosts;
    public int GetUpgradeCostFrom(int level)
    {
        if (level < 0 || level > _maxLevel)
        {
            return -1;
        }
        return _shipUpgradeCosts[level];
    }
}
