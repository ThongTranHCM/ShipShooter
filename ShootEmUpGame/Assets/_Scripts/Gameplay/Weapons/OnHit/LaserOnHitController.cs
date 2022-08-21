using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;
using UnityEngine;

public class LaserOnHitController : OnHitController
{
    Collider2D[] arrayCollider = new Collider2D[40];
    Vector2 laserPos;
    Vector2 laserSize;

    [SerializeField]
    private float _laserWidth;

    public void Start()
    {
        laserPos = Vector2.zero;
        laserSize = new Vector2(_laserWidth, Constants.SizeOfCamera().y * 2);
    }

    public override void OnApplyOnHit(IEnemyController enemy)
    {
        IEnemyController targetEnemy;
        laserPos.x = enemy.transform.position.x;
        int count = Physics2D.OverlapBoxNonAlloc(laserPos, laserSize, 0, arrayCollider);
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
