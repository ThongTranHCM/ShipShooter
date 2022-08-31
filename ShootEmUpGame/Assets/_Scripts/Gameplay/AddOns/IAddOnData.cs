using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IAddOnData
{
    [SerializeField]
    private AddOnEquipData.AddOnType _addOnType;
    public AddOnEquipData.AddOnType GetAddOnType
    {
        get { return _addOnType; }
    }
    [SerializeField]
    private Sprite _addOnSpr;
    public Sprite GetSprite { get { return _addOnSpr; } }
    [SerializeField]
    private float _baseValue;
    [SerializeField]
    private float _increaseValue;
    [SerializeField]
    private GameObject _spawnGun;
    [SerializeField]
    private DOGunData _spawnGunData;
    [SerializeField]
    private OnHitController _onHitController;
    public OnHitController OnHitController { get { return _onHitController; } }
    [SerializeField]
    private PowerUpAddOnController _powerUpController;
    public PowerUpAddOnController powerUpController { get { return _powerUpController; } }
    [SerializeField]
    private bool _isUnlocked;
    public bool IsUnlocked { get { return _isUnlocked; } }

    public float GetLevel { get { return DataManager.Instance.addOnUserData.GetAddOnInfo(_addOnType).CurrentLevel; } }
    public float GetValue { get { return _baseValue + _increaseValue * GetLevel; } }
    public bool HasGun { get { return _spawnGun != null; } }
    public bool HasOnHit { get { return _onHitController != null; } }
    public bool HasPowerUp { get { return _powerUpController != null; } }

    public IGunController InstallGun()
    {
        GameObject gunObject = (GameObject)GameObject.Instantiate(_spawnGun);
        IGunController iGunController = gunObject.GetComponent<IGunController>();
        iGunController.CopyGunData(_spawnGunData);
        iGunController.Install(GetValue, (!HasOnHit && !HasPowerUp));
        return iGunController;
    }
    public OnHitController InstallOnHit()
    {
        OnHitController onHitController = GameObject.Instantiate(_onHitController);
        return onHitController;
    }
    public PowerUpAddOnController InstallPowerUp()
    {
        PowerUpAddOnController powerUpController = GameObject.Instantiate(_powerUpController);
        return powerUpController;
    }
}
