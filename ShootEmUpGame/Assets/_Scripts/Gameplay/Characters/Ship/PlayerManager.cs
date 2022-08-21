using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;
public class PlayerManager : MonoBehaviour
{
    public IShipController _shipController;
    public AddOnEquipData addOnEquipData;
    public OnHitManager _onHitManager;
    private bool _isInstalled;
    private int _iLives;
    [SerializeField]
    private DOGunData _gunData;
    IEnumerator DoActionInstall()
    {
        _iLives = GamePlayManager.Instance.Level.PlayerHP;
        _shipController.gunController.CopyGunData(_gunData);
        _shipController.gunController.Install();
        InstallAddOns();
        SetLives(_iLives);
        yield return null;
    }
    public Coroutine Install()
    {
        if (_isInstalled)
        {
            return null;
        }
        return StartCoroutine(DoActionInstall());
    }
    public void InstallAddOns()
    {
        List<IAddOnData> addOns = addOnEquipData.getEquipAddOn();
        for (int i = 0; i < addOns.Count; i++)
        {
            InstallAddOn(addOns[i]);
        }
    }

    public void InstallAddOn(AddOnEquipData.AddOnType addOnType)
    {
        IAddOnData addOnData = GameInformation.Instance.addOnEquipData.GetAddOnData(addOnType);
        InstallAddOn(addOnData);
    }

    public void InstallAddOn(IAddOnData addOnData)
    {
        IGunController gunController = null;
        OnHitController onHitController = null;
        PowerUpAddOnController powerUpController = null;
        if (addOnData.HasGun)
        {
            gunController = addOnData.InstallGun();
            gunController.transform.SetParent(_shipController.transform);
            gunController.transform.localPosition = Vector3.zero;
        }
        if (addOnData.HasOnHit)
        {
            onHitController = addOnData.InstallOnHit();
            _onHitManager.AddOnHit(onHitController);
            onHitController.GunController = gunController;
        }
        if (addOnData.HasPowerUp)
        {
            powerUpController = addOnData.InstallPowerUp();
            _onHitManager.AddPowerUpAddOn(powerUpController);
            powerUpController.GunController = gunController;
        }
    }

    public void SetLives(int lives)
    {
        _iLives = lives;
        _shipController.InstallHealth(_iLives);
        GamePlayManager.Instance.UIManager.InstallLives(_iLives);
    }
    public void OnEnemyGetDamage(IEnemyController enemy, float damage, ApplyEffectData.DamageSource damageSource)
    {
        _onHitManager.OnDamageEnemy(damage, enemy, damageSource);
    }
    public void OnKillEnemy(IEnemyController enemy)
    {
        _onHitManager.OnKillEnemy(enemy);
    }
}
