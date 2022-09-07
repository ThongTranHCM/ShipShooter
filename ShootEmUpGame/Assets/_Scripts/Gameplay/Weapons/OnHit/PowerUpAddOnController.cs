using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;
using GamePoolManager;

public class PowerUpAddOnController : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemPrefab;
    private float _damageCount;
    [SerializeField]
    private float _totalDamageToTrigger;
    public DamageTypeRatio damageRatio;
    protected IGunController _gunController;
    [SerializeField]
    private float _gunDurationActive;
    private float _gunActiveTime;
    private bool _gunIsActive;
    public IGunController GunController
    {
        set { _gunController = value; }
    }

    public bool canCreatePowerUp { get { return _damageCount > _totalDamageToTrigger; } }


    public bool IncreaseTrackingDamage(float damage, ApplyEffectData.DamageSource damageSource)
    {
        bool result = false;
        _damageCount += damage * damageRatio.GetRatio(damageSource);
        if (_damageCount > _totalDamageToTrigger)
        {
            result = true;
        }
        return result;
    }
    public void CreatePowerUp(IEnemyController enemy)
    {
        if (_itemPrefab != null && canCreatePowerUp)
        {
            _damageCount -= _totalDamageToTrigger;
            Transform effectTf = PoolManager.Pools[Constants.poolOnHitEffect].Spawn(_itemPrefab, enemy.transform.position, Quaternion.identity).transform;
            ItemController item = effectTf.GetComponent<ItemController>();
            item.onCollide += OnPowerUpConsume;
        }
    }

    public virtual void OnPowerUpConsume() {
        _gunController.SetUpPlay();
        _gunActiveTime = Time.time;
        _gunIsActive = true;
    }

    public void Update()
    {
        if (Time.time > _gunActiveTime + _gunDurationActive)
        {
            if (_gunIsActive)
            {
                _gunController.SetUpStop(null, false);
                _gunIsActive = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnPowerUpConsume();
        }
    }
}
