using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class OnHitManager : MonoBehaviour
{
    private List<OnHitController> _listOnHitController = new List<OnHitController>();
    private List<PowerUpAddOnController> _listPowerUpAddOn = new List<PowerUpAddOnController>();
    public void AddOnHit(OnHitController onHitController)
    {
        _listOnHitController.Add(onHitController);
    }
    public void RemoveOnHit(int index)
    {
        _listOnHitController.RemoveAt(index);
    }
    public void AddPowerUpAddOn(PowerUpAddOnController powerUpAddOn)
    {
        _listPowerUpAddOn.Add(powerUpAddOn);
    }
    public void RemovePowerUpAddOn(int index)
    {
        _listPowerUpAddOn.RemoveAt(index);
    }

    public void OnDamageEnemy(float damage, IEnemyController enemy, ApplyEffectData.DamageSource damageSource)
    {
        OnHitController onHitController;
        for (int i = 0; i < _listOnHitController.Count; i++)
        {
            onHitController = _listOnHitController[i];
            if (onHitController.IncreaseTrackingDamage(damage, damageSource))
            {
                onHitController.CreateEffect(enemy);
                onHitController.OnApplyOnHit(enemy);
            }
        }
        PowerUpAddOnController powerUpAddOn;
        for (int i = 0; i < _listPowerUpAddOn.Count; i++)
        {
            powerUpAddOn = _listPowerUpAddOn[i];
            powerUpAddOn.IncreaseTrackingDamage(damage, damageSource);
        }
    }

    public void OnKillEnemy(IEnemyController enemy)
    {
        PowerUpAddOnController powerUpAddOn;
        for (int i = 0; i < _listPowerUpAddOn.Count; i++)
        {
            powerUpAddOn = _listPowerUpAddOn[i];
            if (powerUpAddOn.canCreatePowerUp)
            {
                powerUpAddOn.CreatePowerUp(enemy);
            }
        }
    }
}
