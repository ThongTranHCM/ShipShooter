using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;

public class StaticChargeGunController : IGunController
{
    private static StaticChargeGunController _instance;
    public Transform _tfVisual;
    public static StaticChargeGunController Instance
    {
        get { return _instance; }
    }
    [SerializeField]
    private FindTargetController _findTarget;
    private int _numberOfCharges = 0;
    [SerializeField]
    private float _chargeTime;
    [SerializeField]
    private float _showTime;
    [SerializeField]
    private float _distance;
    private float _lastTime;

    public void Start()
    {
        _numberOfCharges = 0;
        _lastTime = -_chargeTime;
        _instance = this;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_numberOfCharges > 0 && _lastTime + _chargeTime < Time.time)
        {
            ShootCharge();
        }
        if (_lastTime + _showTime < Time.time && _tfVisual.gameObject.activeSelf)
        {
            _tfVisual.gameObject.SetActive(false);
        }
    }

    public void IncreaseCharges()
    {
        _numberOfCharges++;
    }

    public void ShootCharge()
    {
        GameObject target = _findTarget.FindTarget();
        if (target == null) return;
        float distance = (target.transform.position - transform.position).magnitude;
        if (distance < _distance)
        {
            TopDownCharacterController character = target.GetComponent<TopDownCharacterController>();
            if (character != null)
            {
                Vector3 diff = character.transform.position - _tfVisual.transform.position;
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                _tfVisual.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                _tfVisual.gameObject.SetActive(true);
                _tfVisual.localScale = new Vector3(1, distance, 1);
                character.SetUpHit(gunData.bulletData, character.transform.position);
                _lastTime = Time.time;
                _numberOfCharges--;
            }
        }
    }
}
