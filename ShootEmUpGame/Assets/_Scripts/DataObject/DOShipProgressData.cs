using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "shipProressData", menuName = "Shooter/ShipProgressData")]
public class DOShipProgressData : ScriptableObject
{
    public int shipCost;
    public int maxLevel;
    public List<int> shipPowers;
    public List<int> shipUpgradeCosts;
}
