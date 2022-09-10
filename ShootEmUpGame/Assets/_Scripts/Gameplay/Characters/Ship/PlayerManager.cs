using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;
public class PlayerManager : MonoBehaviour
{
    public IShipController _shipController;
    public GameObject shipObject
    {
        get { return _shipController.gameObject; }
    }
    public AddOnEquipData addOnEquipData;
    public OnHitManager _onHitManager;
    private bool _isInstalled;
    private int _iLives;
    [SerializeField]
    private DOGunData _gunData;
    IEnumerator DoActionInstall(int shipIndex)
    {
        _iLives = GamePlayManager.Instance.Level.PlayerHP;
        int level = DataManager.Instance.selectedShipLevel;
        _shipController.Install(shipIndex, level);
        _shipController.gunController.CopyGunData(_gunData);
        _shipController.gunController.Install(true);
        InstallAddOns();
        SetLives(_iLives);
        yield return null;
    }
    public Coroutine Install(int shipIndex)
    {
        if (_isInstalled)
        {
            return null;
        }
        GameObject shipObject = Instantiate<GameObject>(GameInformation.Instance.shipData[shipIndex].shipObjectPrefab, transform);
        _shipController = shipObject.GetComponent<IShipController>();
        return StartCoroutine(DoActionInstall(shipIndex));
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
