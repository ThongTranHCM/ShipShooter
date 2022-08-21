using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class BlizzardOnHitController : OnHitController
{
    public ApplyEffectData slowEffectData;
    public override void OnApplyOnHit(IEnemyController enemy)
    {
        if (enemy.isSlow)
        {
            enemy.CallEventHit(_effectData);
        }
        else
        {
            enemy.CallEventHit(slowEffectData);
        }
    }
}
