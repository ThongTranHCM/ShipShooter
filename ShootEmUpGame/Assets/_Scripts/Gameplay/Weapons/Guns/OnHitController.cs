using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;
using GamePoolManager;

public class OnHitController : MonoBehaviour
{
    protected IGunController _gunController;
    public IGunController GunController
    {
        set { _gunController = value; }
    }
    public DamageTypeRatio damageRatio;
    public float DamageToTrigger
    {
        get { return damageRatio.triggerDamageRatio/100f * IShipController.ShipPower; }
    }
    [SerializeField]
    private GameObject _onHitObject;
    [SerializeField]
    protected ApplyEffectData _effectData;
    private float _damageCount;
    public bool IncreaseTrackingDamage(float damage, ApplyEffectData.DamageSource damageSource)
    {
        bool result = false;
        _damageCount += damage * damageRatio.GetRatio(damageSource);
        if (_damageCount > DamageToTrigger)
        {
            _damageCount -= DamageToTrigger;
            result = true;
        }
        return result;
    }
    public void CreateEffect(IEnemyController enemy)
    {
        if (_onHitObject != null)
        {
            Transform effectTf = PoolManager.Pools[Constants.poolOnHitEffect].Spawn(_onHitObject, enemy.transform.position, Quaternion.identity).transform;
            OnHitEffect onHitEffect = effectTf.GetComponent<OnHitEffect>();
            onHitEffect.Install(enemy);
        }
    }

    public virtual void OnApplyOnHit(IEnemyController enemy) { }
}
