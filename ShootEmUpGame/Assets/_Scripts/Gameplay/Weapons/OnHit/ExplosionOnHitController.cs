using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class ExplosionOnHitController : OnHitController
{
    Collider2D[] arrayCollider = new Collider2D[40];

    [SerializeField]
    private float _explosionSize;
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionSize);
    }
#endif

    public override void OnApplyOnHit(IEnemyController enemy)
    {
        IEnemyController targetEnemy;
        int count = Physics2D.OverlapCircleNonAlloc((Vector2)enemy.transform.position, _explosionSize, arrayCollider);
        for (int i = 0; i < count; i++)
        {
            Collider2D collider = arrayCollider[i];
            if (collider != null)
            {
                targetEnemy = collider.transform.GetComponent<IEnemyController>();
                if (targetEnemy != null)
                {
                    targetEnemy.CallEventHit(_effectData);
                }
            }
        }
    }
}
