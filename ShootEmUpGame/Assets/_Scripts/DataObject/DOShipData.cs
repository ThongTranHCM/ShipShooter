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
}
