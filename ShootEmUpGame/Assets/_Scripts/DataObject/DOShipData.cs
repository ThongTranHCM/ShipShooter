using UnityEngine;


[CreateAssetMenu(fileName = "shipData_0", menuName = "Shooter/ShipData")]
public class DOShipData : ScriptableObject
{
    public string shipName;
    public float basePower;
    public float increasePowerValue;
    public GameObject shipObjectPrefab;

    public Sprite spritePresentShip;
    public Mesh meshShip;
    public Material materialShip;

    [SerializeField]
    private float _buyCost;
    public float ShipCost { get { return _buyCost; } }
    [SerializeField]
    private string _buyCurrency;
    public string ShipCostCurrency { get { return _buyCurrency; } }
    [SerializeField]
    private int _maxLevel;
    public int MaxLevel { get { return _maxLevel; } }
    [SerializeField]
    private float[] _upgradeCosts;
    public float GetUpgradeCostFrom(int level)
    {
        if (level < 0 || level > _maxLevel)
        {
            return -1;
        }
        return _upgradeCosts[level];
    }
    [SerializeField]
    private float[] _shipPowers;
    public float GetPower(int level)
    {
        if (level < 0 || level > _maxLevel)
        {
            return level = 0;
        }
        return _shipPowers[level];
    }
}
