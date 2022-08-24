using ThongNguyen.PlayerController;
using UnityEngine;

[CreateAssetMenu(fileName = "gunData_0", menuName = "Shooter/GunData")]
public class DOGunData : ScriptableObject
{
    public float attackSpeed;

    public int numWavePerShot = 0;
    public int numOfBulletPerWave;
    public float timeDelayForEachWave; // 1 lần bắn có thể bắt ra nhiều đợt (như AK)
    public BulletData bulletData;

    public DOGunData() { }
    public DOGunData(DOGunData other) {
        Init(other);
    }
    public void Init(DOGunData other)
    {
        attackSpeed = other.attackSpeed;
        //numberOfShot = other.numberOfShot;
        numWavePerShot = other.numWavePerShot;
        numOfBulletPerWave = other.numOfBulletPerWave;
        timeDelayForEachWave = other.timeDelayForEachWave;
        if (bulletData == null)
        {
            bulletData = new BulletData(other.bulletData);
        }
        else
        {
            bulletData.Init(other.bulletData);
        }
    }
}

[System.Serializable]
public class BulletData
{
    protected Vector2 destination;
    public float speed;
    public float timeToMove;
    public bool canPenetrated;
    [Header("Homing")]
    public GameObject _targetLocked;
    protected float zAngle;
    public float rotSpeed; // tốc độ chuyển hướng unit/s. Nếu = -1 thì sẽ quay compass ngay lập tức
    public float timeFollowTarget; // = -1 là dí tới khi mục tiêu bị tiêu diệt

    [SerializeField]
    private ApplyEffectData _applyEfect;
    public ApplyEffectData ApplyEfect { get { return _applyEfect; } }
    private bool isInstalled;

    public BulletData() { }
    public BulletData(BulletData other)
    {
        Init(other);
    }

    public virtual void Init(BulletData otherBulletData)
    {
        if (otherBulletData == null || isInstalled)
        {
            return;
        }
        isInstalled = true;
        speed = otherBulletData.speed;
        timeToMove = otherBulletData.timeToMove;
        canPenetrated = otherBulletData.canPenetrated;
        if (_applyEfect == null)
        {
            _applyEfect = new ApplyEffectData();
        }
        _applyEfect.Init(otherBulletData._applyEfect);
    }
}

[System.Serializable]
public class ApplyEffectData
{
    public enum DamageSource
    {
        PlayerGun, PlayerOnHit, PlayerPowerUp,

        FromMonster = 1000 //Usually shouldn't check DamageSource in the first place
    }
    [Header("Damage")]
    [SerializeField]
    public float attackPower;

    [SerializeField]
    public float ratioDamage;

    [SerializeField]
    public float criticalRate;

    [SerializeField]
    public float criticalDamage;

    public DamageSource damageSource;

    #region Callback
    public System.Action callbackShotMiss;
    public System.Action<GameObject> callbackHitEnemy;
    public System.Action<IEnemyController> callbackKillEnemy;
    public System.Action<Transform, Vector2> callbackShowEffectHit;
    public System.Action<Transform, Vector2> callbackShowEffectCritical;
    public System.Action callbackSelfDestruction;
    #endregion
    #region Effects
    public bool canFreeze { get { return timeFreeze > 0; } }
    public float freezeChance;
    public float timeFreeze;
    public bool canSlow { get { return timeSlow > 0; } }
    public float slowChance;
    public float timeSlow;
    public float slowPercent;
    #endregion

    public void Init(ApplyEffectData otherEffectData)
    {
        attackPower = otherEffectData.attackPower;
        criticalDamage = otherEffectData.criticalDamage;
        criticalRate = otherEffectData.criticalRate;
        ratioDamage = otherEffectData.ratioDamage;
        damageSource = otherEffectData.damageSource;
        callbackSelfDestruction = otherEffectData.callbackSelfDestruction;
        freezeChance = otherEffectData.freezeChance;
        timeFreeze = otherEffectData.timeFreeze;
        slowChance = otherEffectData.slowChance;
        slowPercent = otherEffectData.slowPercent;
    }

    public virtual float GetTotalDamage(IEnemyController target)
    {
        float totalDamage = attackPower;
        float damageCritical = 0;
        // -----------------------------------------------------
        float totalCriticalChance = criticalRate;
        damageCritical = (int)(totalCriticalChance / 100f) * criticalDamage;
        float randCritical = Random.Range(0, 100);
        if (randCritical < (totalCriticalChance) % 100f)
        {
            damageCritical += criticalDamage;
        }
        // -----------------------------------------------------
        totalDamage += damageCritical;
        if (totalDamage < 1f)
        {
            totalDamage = 1f;
        }
        totalDamage += ratioDamage * IShipController.ShipPower;
        return totalDamage;
    }
}

[System.Serializable]
public class DamageTypeRatio
{
    public ApplyEffectData.DamageSource[] damageSources;
    [Tooltip("0 for none. 100 for full damage.")]
    public float[] ratios;
    public float triggerDamageRatio;

    public float GetRatio(ApplyEffectData.DamageSource source)
    {
        if (source >= ApplyEffectData.DamageSource.FromMonster)
        {
            return 0;
        }
        float result = 1;
        for (int i = 0; i < damageSources.Length; i++)
        {
            if (damageSources[i].Equals(source))
            {
                result = ratios[i] / 100f;
                break;
            }
        }
        return result;
    }
}