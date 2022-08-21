using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class StaticOnHitController : OnHitController
{
    private StaticChargeGunController _staticChargeGunController;
    public void Start()
    {
        _staticChargeGunController = _gunController as StaticChargeGunController;
    }
    public override void OnApplyOnHit(IEnemyController enemy)
    {
        _staticChargeGunController.IncreaseCharges();
    }
}
