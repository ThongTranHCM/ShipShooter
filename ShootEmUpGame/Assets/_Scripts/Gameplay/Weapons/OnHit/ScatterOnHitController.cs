using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class ScatterOnHitController : OnHitController
{
    public override void OnApplyOnHit(IEnemyController enemy)
    {
        _gunController.MoveRotateOutwardOfTarget(enemy.transform, IShipController.Instance.transform);
        _gunController.ShootObjects();
        _gunController.ShootParticles();
    }
}
