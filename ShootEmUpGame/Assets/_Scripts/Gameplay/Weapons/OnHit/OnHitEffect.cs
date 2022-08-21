using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;
using GamePoolManager;

public class OnHitEffect : MonoBehaviour
{
    private float _startTime;
    [SerializeField]
    private float _lifeTime = 2;
    

    public virtual void Install(IEnemyController enemy)
    {
        _startTime = Time.time;
    }

    public void Update()
    {
        if (Time.time - _startTime > _lifeTime)
        {
            PoolManager.Pools[Constants.poolOnHitEffect].Despawn(transform);
        }
    }
}
