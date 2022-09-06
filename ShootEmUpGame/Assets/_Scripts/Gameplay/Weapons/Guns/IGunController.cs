using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;

public class IGunController : MonoBehaviour
{
    public enum GunState
    {
        none, shot
    }
    public BulletParticleSystem[] bulletParticleSystems;
    public BulletController[] bulletControllers;
    public Transform[] shootTransforms;
    public bool isRandomPosition;
    public bool stopGunAtInstall;
    public float sizeX, sizeY;
    protected DOGunData gunData;
    public System.Action<GameObject, BulletData> callbackHitEnemy;

    private bool isInstalled;
    //Orientate
    [SerializeField]
    private bool _isOrientate;
    [SerializeField]
    private bool _isOrientateOncePerWave;
    [SerializeField]
    private bool _isOrientateOncePerShot;
    private float[] _shootAngles;
    private float _tmpTimeDelay;
    private int _tmpShootPhase;
    private int _tmpWaveId;
    private Vector3 bulletDeltaPosition;
    [SerializeField]
    private GameObject _bulletPrefab;
    private GunState _state;
    private bool needSyncTime = false;

    private System.Action callbackAllGunsEmptyParticle;

    public float BaseDamage()
    {
        return gunData.bulletData.ApplyEfect.attackPower;
    }

    public float BaseCritRate()
    {
        return gunData.bulletData.ApplyEfect.criticalRate;
    }

    public float BulletSpeed()
    {
        return gunData.bulletData.speed;
    }

    public void CopyGunData(DOGunData targetData)
    {
        if (gunData == null)
        {
            gunData = new DOGunData(targetData);
        }
        else
        {
            gunData.Init(targetData);
        }
    }

    public virtual void Install(float ratioDamage, bool syncTime = true)
    {
        gunData.bulletData.ApplyEfect.ratioDamage = ratioDamage;

        _shootAngles = new float[shootTransforms.Length];
        for (int i = 0; i < shootTransforms.Length; i++)
        {
            _shootAngles[i] = shootTransforms[i].transform.localEulerAngles.z;
        }
        Install(syncTime);
    }

    public virtual void Install(bool syncTime)
    {
        if (isInstalled)
        {
            return;
        }
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            bulletParticleSystems[i].Install(gunData, !stopGunAtInstall);
        }
        for (int i = 0; i < bulletControllers.Length; i++)
        {
            bulletControllers[i].Install(this, gunData.bulletData);
        }
        if (syncTime)
        {
            needSyncTime = syncTime;
            for (int i = 0; i < bulletParticleSystems.Length; i++)
            {
                bulletParticleSystems[i].myParticleSystem.Simulate(GamePlayManager.Instance.PlayerManager._shipController.gunController.bulletParticleSystems[0].myParticleSystem.time, false, true);
            }
        }
        isInstalled = true;
        if (!stopGunAtInstall)
        {
            SetUpPlay();
        }
    }

    public void SetUpStop(System.Action callback, bool doUninstall)
    {
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            bulletParticleSystems[i].StopEmit(() => CallbackParticleSystemsEmptyParticles(doUninstall), true);
        }
        _state = GunState.none;
        callbackAllGunsEmptyParticle = callback;
    }

    public void SetUpPlay()
    {
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            bulletParticleSystems[i].CancelStopEmit();
        }
        callbackAllGunsEmptyParticle = null;
        if (gunData.numWavePerShot > 0)
        {
            _tmpShootPhase = 0;
            _tmpTimeDelay = 0;
            _tmpWaveId = 0;
        }
        else
        {
            for (int i = 0; i < bulletParticleSystems.Length; i++)
            {
                bulletParticleSystems[i].StartEmit();
            }
        }
        Debug.LogError(" SetUpGun Play ");
        _state = GunState.shot;
    }

    public void CallbackParticleSystemsEmptyParticles(bool doUninstall)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            if (bulletParticleSystems[i].myParticleSystem.particleCount > 0)
            {
                return;
            }
        }
        if (doUninstall)
        {
            isInstalled = false;
        }
        callbackAllGunsEmptyParticle?.Invoke();
        callbackAllGunsEmptyParticle = null;
    }

    public void MoveRotateOutwardOfTarget(Transform target, Transform fromPos)
    {
        Vector3 delta = (target.position - fromPos.position);
        bulletDeltaPosition = delta.normalized;
        transform.position = target.position + delta.normalized;
        float zDegAngle = (Mathf.Atan2(delta.y, delta.x) - Mathf.PI / 2) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, zDegAngle);
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            ParticleSystem particleSystem = bulletParticleSystems[i].myParticleSystem;
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.startRotationZ = (zDegAngle + _shootAngles[i]) * Mathf.Deg2Rad;
        }
    }

    public virtual void FixedUpdate()
    {
        if (_state.Equals(GunState.shot))
        {
            if (needSyncTime)
            {
                needSyncTime = false;
                for (int i = 0; i < bulletParticleSystems.Length; i++)
                {
                    bulletParticleSystems[i].myParticleSystem.time = GamePlayManager.Instance.PlayerManager._shipController.gunController.bulletParticleSystems[0].myParticleSystem.time;
                }
            }
            if (gunData.numWavePerShot > 0)
            {
                switch (_tmpShootPhase)
                {
                    case 0: //Shoot
                        if (_isOrientateOncePerWave)
                        {
                            for (int j = 0; j < bulletParticleSystems.Length; j++)
                            {
                                bulletParticleSystems[j].Orientate();
                            }
                        }
                        ShootParticles();
                        ShootObjects();
                        if (_tmpWaveId + 1 >= gunData.numWavePerShot)
                        {
                            _tmpShootPhase = 2;
                        }
                        else
                        {
                            _tmpShootPhase = 1;
                        }
                        break;
                    case 1: //WaitForWave
                        _tmpTimeDelay += Time.fixedDeltaTime;
                        if (_tmpTimeDelay > gunData.timeDelayForEachWave)
                        {
                            _tmpTimeDelay -= gunData.timeDelayForEachWave;
                            _tmpShootPhase = 0;
                            _tmpWaveId++;
                        }
                        break;
                    case 2: //WaitForShot
                        _tmpTimeDelay += Time.fixedDeltaTime;
                        if (_tmpTimeDelay > 1 / gunData.attackSpeed)
                        {
                            _tmpTimeDelay -= 1 / gunData.attackSpeed;
                            _tmpShootPhase = 0;
                            _tmpWaveId = 0;

                            if (_isOrientateOncePerShot)
                            {
                                for (int j = 0; j < bulletParticleSystems.Length; j++)
                                {
                                    bulletParticleSystems[j].Orientate();
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                _tmpTimeDelay += Time.fixedDeltaTime;
                if (_tmpTimeDelay > 1 / gunData.attackSpeed)
                {
                    _tmpTimeDelay -= 1 / gunData.attackSpeed;

                    if (_isOrientate)
                    {
                        for (int j = 0; j < bulletParticleSystems.Length; j++)
                        {
                            bulletParticleSystems[j].Orientate();
                        }
                    }
                    ShootObjects();
                }
            }
        }
        else
        {
            if (_isOrientate)
            for (int j = 0; j < bulletParticleSystems.Length; j++)
            {
                bulletParticleSystems[j].Orientate();
            }
        }
    }
    
    public void ShootParticles()
    {
        for (int i = 0; i < bulletParticleSystems.Length; i++)
        {
            bulletParticleSystems[i].Emit();
        }
    }

    public void ShootObjects()
    {
        if (_bulletPrefab != null)
        {
            Vector3 positionSpawn;
            Vector3 positionDestination;
            Vector3 topLeftPos = Constants.GetTopLeftScreen() + new Vector2(sizeX, sizeY);
            Vector3 botRightPos = Constants.GetBottomRightScreen() - new Vector2(sizeX, sizeY);
            for (int i = 0; i < shootTransforms.Length; i++)
            {
                positionSpawn = transform.position;
                positionDestination = transform.position + bulletDeltaPosition;
                if (isRandomPosition)
                {
                    positionSpawn = new Vector3(Random.Range(topLeftPos.x, botRightPos.x), Random.Range(topLeftPos.y, botRightPos.y), 0);
                    positionDestination = positionSpawn;
                }
                BulletController bullet = CreateBullet(positionSpawn, positionDestination, shootTransforms[i].rotation);
                bullet.SetUpFollow(null, transform.eulerAngles.z);
            }
        }
    }

    protected virtual BulletController CreateBullet(Vector3 position, Vector3 destination, Quaternion rotation)
    {
        string poolNameOfBullet = Constants.poolNameBulletsOfPlayer;
        Transform tfBullet = PoolManager.Pools[poolNameOfBullet].Spawn(_bulletPrefab.transform, position, rotation);

        BulletController bullet = tfBullet.GetComponent<BulletController>();
        bullet.Install(this, gunData.bulletData, position, destination);
        return bullet;
    }

    protected virtual BulletController CreateBullet(Quaternion rotation)
    {
        Vector2 position = transform.position;
        Vector2 destination = transform.position + bulletDeltaPosition;

        Vector3 topLeftPos = Constants.GetTopLeftScreen() + new Vector2(sizeX, sizeY);
        Vector3 botRightPos = Constants.GetBottomRightScreen() - new Vector2(sizeX, sizeY);
        if (isRandomPosition)
        {
            position = new Vector3(Random.Range(topLeftPos.x, botRightPos.x), Random.Range(topLeftPos.y, botRightPos.y), 0);
            destination = position;
        }
        string poolNameOfBullet = Constants.poolNameBulletsOfPlayer;
        Transform tfBullet = PoolManager.Pools[poolNameOfBullet].Spawn(_bulletPrefab.transform, position, rotation);

        BulletController bullet = tfBullet.GetComponent<BulletController>();
        bullet.Install(this, gunData.bulletData, position, destination);
        return bullet;
    }
}