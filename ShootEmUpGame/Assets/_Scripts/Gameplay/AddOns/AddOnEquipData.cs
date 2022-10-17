using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "addOnEquipData", menuName = "Shooter/AddOnEquipData")]
public class AddOnEquipData : ScriptableObject
{
    public enum AddOnType
    {
        None = 0,
        GunSplit = 10,
        GunEnergyOrb,
        GunLaser,
        GunPlasma,
        PowerUpHoming = 100,  //Kill enemy has a chance to drop Homing Missiles. Collect Homing Missiles to fire N Homing Missiles to the Enemy.
        PowerUpCoverFire,     //Kill enemy has a chance to drop Cover Fire.
        PowerUpFlashBomb,     //Kill enemy has a chance to drop Flash Bomb. Collect Flash Bomb to deal damage to all enemy on the screen and knock them back.
        PowerUpBlackHole,     //Kill enemy has a chance to drop Black Hole.
        PowerUpMegaShot,      //Kill enemy has a chance to drop MegaShot. Collect MegaShot to attack MegaShot SkyChamp style.
        PowerUpXCross,        //Kill enemy has a chance to drop XCross. Shoot 2 big penetrating bullets.
        PowerUpVertLaser,     //Kill enemy has a chance to drop Vert Laser. Shoot Vertical Laser at random locations 
        OnHitExplosion = 200, //Damaging enemy has a chance to create an Explosion, dealing damage to all enemy in a small area.
        OnHitScatter,         //Damaging enemy has a chance to create N Homing Missiles from the main ship.
        OnHitBlizzard,        //Damaging enemy has a chance to create a Blizzard. Enemies inside the Blizzard will be slowed by X%, take damage per second.
        OnHitStaticCharge,    //Damaging enemy has a chance to create Static Charge. If there is an enemy nearby, consume the Static Charge to deal damage to that enemy.
        OnHitVertLaser,
        HpValiantHeart = 300, //Increase HP by 1, increase your Attack Power by X% when you are at full health.
        LostHpSlowmo = 400,   //Whenever you lose HP, slow enemies and their projectiles by X% for N seconds
        LoseHpBerserk,        //When you lose HP, increase your Attack Power for N seconds.
        LoseHpDesparate       //When you lose HP, deal massive damage to all enemies. 
    }


    public List<IAddOnData> addOnDatas;
    [SerializeField]
    private int _maxLevel;
    public int MaxLevel { get { return _maxLevel; } }
    [SerializeField]
    private List<int> _upgradeFragmentCosts;
    [SerializeField]
    private List<float> _levelPowers;
    [SerializeField]
    private int _unlockFragmentCost;

    public List<IAddOnData> getEquipAddOn()
    {
        List<IAddOnData> addOnList = new List<IAddOnData>();
        if (DataManager.Instance != null)
        {
            List<string> listAddOns = DataManager.Instance.addOnUserData.GetListAddOnEquiped();
            IAddOnData addOnData = null;
            addOnData = GetAddOnData(listAddOns[0]);
            if (addOnData != null)
            {
                addOnList.Add(addOnData);
            }
            addOnData = GetAddOnData(listAddOns[1]);
            if (addOnData != null)
            {
                addOnList.Add(addOnData);
            }
            addOnData = GetAddOnData(listAddOns[2]);
            if (addOnData != null)
            {
                addOnList.Add(addOnData);
            }
            addOnData = GetAddOnData(listAddOns[3]);
            if (addOnData != null)
            {
                addOnList.Add(addOnData);
            }
        }
        return addOnList;
    }

    public List<string> GetUnlockableAddOnList()
    {
        List<string> result = new List<string>();
        string stringA = "";
        for (int i = 0; i < addOnDatas.Count; i++)
        {
            if (addOnDatas[i].IsUnlocked)
            {
                stringA = addOnDatas[i].GetAddOnType.ToString();
                result.Add(stringA);
            }
        }
        return result;
    }

    public IAddOnData GetAddOnData(AddOnType addOnType)
    {
        IAddOnData result = null;
        for (int i = 0; i < addOnDatas.Count; i++)
        {
            if (addOnDatas[i].GetAddOnType.Equals(addOnType) && addOnDatas[i].IsUnlocked)
            {
                result = addOnDatas[i];
                break;
            }
        }
        return result;
    }
    public IAddOnData GetAddOnData(string addOnStr)
    {
        IAddOnData result = null;
        AddOnType addOnType = GetType(addOnStr);
        for (int i = 0; i < addOnDatas.Count; i++)
        {
            if (addOnDatas[i].GetAddOnType.Equals(addOnType) && addOnDatas[i].IsUnlocked)
            {
                result = addOnDatas[i];
                break;
            }
        }
        return result;
    }
    public AddOnType GetType(string addOnStr){
        return (AddOnType)Enum.Parse(typeof(AddOnType), addOnStr);
    }

    public int GetUpgradeCost(int level)
    {
        return _upgradeFragmentCosts[level];
    }

    public int GetUnlockCost()
    {
        return _unlockFragmentCost;
    }
}
