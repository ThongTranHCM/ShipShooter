using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;
using MathUtil;

namespace ThongNguyen.PlayerController
{
    public class IEnemyController : TopDownCharacterController
    {
        [SerializeField]
        private GameObject _hpBarPrefab;
        [SerializeField]
        private CharacterHealthbar _healthBar;
        [SerializeField]
        private Collider2D _enemyCollider;
        private WaveManager _waveInstance;
        private bool _isInstalled;

        //-----Heath and Damage
        private bool _canBeDamaged;
        private bool _affectDamage;
        private float _lastDamage;
        private LowPassFilter _lowPassHealth;
        private LowPassFilter _lowPassHit;
        //private const float _damping = 0.01f;

        //----Effects
        [SerializeField]
        private OnHitEffect _effDiePrefab;
        [SerializeField]
        private bool _spawnCoinAtDie;
        public bool SpawnCoinAtDie { get { return _spawnCoinAtDie; } }
        protected IEnumerator actionFreeze, actionSlow;
        protected float timeFreeze, timeSlow;
        protected float totalPercentSlowSpeed;
        public bool isFreeze
        {
            get
            {
                if (timeFreeze > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool isSlow
        {
            get
            {
                if (timeSlow > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public List<Renderer> myBodyPartRenderers;
        private IGunController gunController;
        protected List<Material> piecesOfBodyMaterial;
        private Color _currentColor = Color.black;
        private Color _currentBrightness = Color.black;
        private MaterialPropertyBlock materialPropertyBlock;
        private float gotHit;
        public bool isRotateTowardShip;
        #region Unity Functions
        protected override void Start()
        {
            base.Start();
            materialPropertyBlock = new MaterialPropertyBlock();
            _lowPassHit = new LowPassFilter(0.025f);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_isInstalled)
            {
                _lowPassHealth.Input(currentHealth);
                _lowPassHit.Input(gotHit);
                //float brightness = (_lowPassHealth.Output() - CurrentHp) / MaxHp;
                float hit = _lowPassHit.Output() / _lowPassHit.GetAlpha();
                this.transform.GetChild(0).transform.localPosition = 0.1f * Vector3.up * hit;
                ChangeBrightness(0.5f * hit * Color.white, 0.5f);
                gotHit = 0;
            }
        }
        #endregion
        public virtual void Install(float hpLevel, float moveSpeed)
        {
            if (_isInstalled)
            {
                return;
            }
            gameObject.SetActive(true);
            CurrentHp = MaxHp = hpLevel;
            _lowPassHealth = new LowPassFilter(0.01f, CurrentHp);
            if (_healthBar == null)
            {
                var hpBar = PoolManager.Pools[Constants.poolMonster].Spawn(_hpBarPrefab);
                _healthBar = hpBar.GetComponent<CharacterHealthbar>();
                _healthBar.Install(transform, hpLevel, hpLevel);
            }
            speed = moveSpeed;
            GamePlayManager.Instance.EnemyContainer.AddEnemy((IEnemyController)this);
            _isInstalled = true;
            _canBeDamaged = true;
        }
        
        public void SetWaveManager(WaveManager WaveInstance)
        {
            _waveInstance = WaveInstance;
        }
        public void InstallGun(GameObject prefab, DOGunData gunData)
        {
            Transform tfGun = PoolManager.Pools[Constants.poolNameEnemyGun].Spawn(prefab.transform, transform.position, transform.rotation, transform);
            gunController = tfGun.GetComponent<IGunController>();
            gunController.CopyGunData(gunData);
            gunController.Install(false);
        }
        protected List<Material> GetPiecesOfBody()
        {
            if (piecesOfBodyMaterial == null)
            {
                piecesOfBodyMaterial = new List<Material>();
                for (int i = 0; i < myBodyPartRenderers.Count; i++)
                {
                    piecesOfBodyMaterial.Add(myBodyPartRenderers[i].material);
                }
            }
            return piecesOfBodyMaterial;
        }

        public void ChangeBodyColor(Color color, float changeDuration)
        {
            if (_currentColor != color)
            {
                _currentColor = color;
                RefreshBodyColor();
            }
        }

        public void ChangeBrightness(Color brightness, float changeDuration){
            if (_currentBrightness != brightness){
                _currentBrightness = brightness;
                RefreshBodyColor();
            }
        }

        protected void RefreshBodyColor()
        {
            for (int i = 0; i < myBodyPartRenderers.Count; i++)
            {
                myBodyPartRenderers[i].GetPropertyBlock(materialPropertyBlock);
                // Assign our new value.
                materialPropertyBlock.SetColor("_LightenColor", _currentColor + _currentBrightness);
                //materialPropertyBlock.SetFloat("_Brightness", _currentBrightness);
                // Apply the edited values to the renderer.
                myBodyPartRenderers[i].SetPropertyBlock(materialPropertyBlock);
            }
        }

        public override void OnUpdateHP()
        {
            if (_healthBar != null) _healthBar.SetCurrentHealth(currentHealth);
        }

        public override float GetSpeed()
        {
            return speed * GetSpeedPercentage();
        }

        public float GetSpeedPercentage(){
            return (1 - totalPercentSlowSpeed /100f);
        }

        protected override void HandleDirection()
        {
            IShipController ship = IShipController.Instance;
            if (ship != null)
            {
                dir = (ship.transform.position - transform.position);
                //ParticleSystem.TriggerModule triggerModule = ship.gunController.myParticleSystem.trigger;
                //triggerModule.SetCollider(0, enemyCollider);
            }
            if (dir.magnitude < 0.2f)
                dir = Vector2.zero;
            else
            {
                dir = dir.normalized;
            }
            if (isRotateTowardShip)
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90);
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            OnEventTriggerEnter2D(other);
        }

        public override void OnParticleTriggerEnter2D(ParticleCollider other)
        {
            if (!_isInstalled)
            {
                return;
            }
            other.OnCollideEnemy(this);
        }

        public virtual void OnEventTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Untagged") || other.transform.parent == null)
            {
                return;
            }

            BulletData bulletData = null;

            BulletController controller = other.transform.parent.gameObject.GetComponent<BulletController>();
            if (controller == null)
            {
                controller = other.gameObject.GetComponent<BulletController>();
            }
            if (controller != null)
            {
                bulletData = controller.bulletData;
            }
            if (bulletData != null)
            {
                SetUpHit(bulletData, other.transform.position);
            }
        }

        public override void SetUpHit(BulletData bulletData, Vector2 posOfHit)
        {
            if (!_isInstalled)
            {
                return;
            }
            if (bulletData == null)
            {
                return;
            }
            CallEventHit(bulletData.ApplyEfect);

            bulletData.ApplyEfect.callbackShowEffectHit?.Invoke(null, posOfHit);

            if (!bulletData.canPenetrated)
            {
                bulletData.ApplyEfect.callbackSelfDestruction?.Invoke();
            }
        }

        public virtual void CallEventHit(ApplyEffectData effectData)
        {
            if (effectData == null)
            {
                return;
            }
            if (_canBeDamaged)
            {
                float totalDamage = 0;
                totalDamage = effectData.GetTotalDamage(this);
                _lastDamage = Time.time;
                _affectDamage = true;
                TakeDamage(effectData, totalDamage);
            }
            SetStatusEffect(effectData);
            if (!Constants.IsOutOfSceneGamePlay(((Vector2)transform.position), new Vector2(1, 1)))
            {
                if (effectData.callbackHitEnemy != null)
                {
                    effectData.callbackHitEnemy(gameObject);
                }
            }
        }
        #region Status Effect
        public void SetStatusEffect(ApplyEffectData effectData)
        {
            if (!gameObject.activeSelf) { return; }
            int isMissStatusEffectCold = 0;
            isMissStatusEffectCold = SetStatusEffectCold(effectData);
        }

        public virtual int SetStatusEffectCold(ApplyEffectData effectData)
        {
            int isMissStatusEffect = 0;
            if (effectData.canSlow)
            {
                isMissStatusEffect = 1;
                int tmpSlowChance = Random.Range(0, 100);
                if (tmpSlowChance < effectData.slowChance)
                {
                    if (effectData.canFreeze)
                    {
                        int tmpFreezingChance = Random.Range(0, 100);
                        if (tmpFreezingChance < effectData.freezeChance)
                        {
                            SetUpFreeze(effectData.timeFreeze);
                        }
                    }
                    SetUpSlow(effectData.timeSlow, effectData.slowPercent);
                    isMissStatusEffect = 2;
                }
            }
            else if (effectData.canFreeze)
            {
                isMissStatusEffect = 1;
                int _tmpFreezingChance = Random.Range(0, 100);
                if (_tmpFreezingChance < effectData.freezeChance)
                {
                    SetUpFreeze(effectData.timeFreeze);
                    isMissStatusEffect = 2;
                }
            }
            return isMissStatusEffect;
        }

        public void SetUpSlow(float duration, float amount)
        {
            if (duration <= timeSlow)
            {
                return;
            }
            if (!isFreeze && !isSlow)
            {
                ChangeBodyColor(new Color(0.1f,0.3f,0.6f), 0.5f);
                if (actionSlow != null)
                {
                    StopCoroutine(actionSlow);
                    actionSlow = null;
                }
                timeSlow = duration;
                totalPercentSlowSpeed = amount;
                actionSlow = DoActionWaitForSlow();
                StartCoroutine(actionSlow);
            }
            else
            {
                timeSlow = duration;
            }
        }
        public void SetUpFreeze(float duration)
        {
            if (!isFreeze && !isSlow)
            {
                if (actionFreeze != null)
                {
                    StopCoroutine(actionFreeze);
                    actionFreeze = null;
                }
                timeFreeze = duration;
                actionFreeze = DoActionWaitForFreeze();
                StartCoroutine(actionFreeze);
            }
        }
        IEnumerator DoActionWaitForFreeze()
        {
            while (timeFreeze > 0)
            {
                yield return null;
                timeFreeze -= Time.deltaTime;
            }
            FinishFreeze();
        }
        IEnumerator DoActionWaitForSlow()
        {
            while (timeSlow > 0)
            {
                yield return null;
                if (!isFreeze)
                {
                    timeSlow -= Time.deltaTime;
                }
            }
            FinishSlow();
        }
        public void FinishFreeze()
        {
            timeFreeze = 0;
            actionFreeze = null;

            if (isSlow)
            {
            }
            else
            {
                ChangeBodyColor(Color.black, 0f);
            }
        }
        public void FinishSlow()
        {
            timeSlow = 0;
            actionSlow = null;

            totalPercentSlowSpeed = 0;
            ChangeBodyColor(Color.black, 0f);
        }
        #endregion
        public override void TakeDamage(ApplyEffectData effectData, float damage)
        {
            float lastHp = currentHealth;
            float currentHp = currentHealth;
            currentHp -= damage;
            if (currentHp <= 0)
            {
                currentHp = 0;
                if (lastHp > 0)
                {
                    OnDie();
                    if (effectData != null
                        && effectData.callbackKillEnemy != null)
                    {
                        effectData.callbackKillEnemy(this);
                    }
                    GamePlayManager.Instance.OnEnemyGetKilled(this);
                }
            }
            GamePlayManager.Instance.OnEnemyGetDamage(this, damage, effectData.damageSource);
            CurrentHp = currentHp;
            gotHit = 1;
        }

        public override void OnDie()
        {
            FinishFreeze();
            FinishSlow();
            //-----Effect when Die
            if (_effDiePrefab != null)
            {
                Transform effectTf = PoolManager.Pools[Constants.poolOnHitEffect].Spawn(_effDiePrefab.transform, transform.position, Quaternion.identity).transform;
                OnHitEffect onHitEffect = effectTf.GetComponent<OnHitEffect>();
                onHitEffect.Install(this);
            }
            //_spawnCoinAtDie Spawn Coin in LevelDesignData DropOnKill
            ChangeBrightness(Color.black,0);
            OnRemove();
        }

        public void OnRemove(){
            if (_healthBar != null)
            {
                PoolManager.Pools[Constants.poolMonster].Despawn(_healthBar.transform);
                _healthBar = null;
            }
            _canBeDamaged = false;
            _affectDamage = false;
             //-----Remove Gun
            if (gunController != null)
            {
                Transform gunTf = gunController.transform;
                PoolManager.Pools[Constants.poolNameEnemyGun].ReparentToGroup(gunTf);
                gunController.SetUpStop(() =>
                {
                    PoolManager.Pools[Constants.poolNameEnemyGun].Despawn(gunTf);
                }, true);
            }
            //-----Disable Self
            GamePlayManager.Instance.EnemyContainer.RemoveEnemy((IEnemyController)this);
            if (_waveInstance != null)
            {
                _waveInstance.OnMonsterDeath(this);
                _waveInstance = null;
            }
            gameObject.SetActive(false);
            PoolManager.Pools[Constants.poolMonster].Despawn(transform);
            _isInstalled = false;
            GamePlayManager.Instance.OnEnemyRemove(this);
        }
    }
}
