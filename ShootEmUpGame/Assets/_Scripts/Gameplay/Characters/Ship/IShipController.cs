using System.Collections;
using UnityEngine;
using MathUtil;

namespace ThongNguyen.PlayerController
{
    public class IShipController : TopDownCharacterController
    {
        public static IShipController Instance
        {
            get { return _instance; }
        }
        private static IShipController _instance;
        public IGunController gunController;
        [Header("Gliding")]
        public float angle;
        [Tooltip("Glide velocity")]
        public float velAngle;
        [Tooltip("Round Angle")]
        public float roundAngle;
        [Tooltip("Max Angle")]
        public float maxAngle;
        public float damping;
        private LowPassFilter _lowPassDir;

        private float _shipPower;
        public float ShipPower
        {
            get { return _shipPower; }
        }

        private int _shipLevel;
        public int ShipLevel
        {
            get { return _shipLevel; }
        }

        private bool _isImmortal;
        protected override void Start()
        {
            base.Start();
            _instance = this;
            if (limitInsideScreen)
            {
                Vector2 cameraSize = Constants.SizeOfCamera();
                _limitLeft = -cameraSize.x / 2;
                _limitRight = cameraSize.x / 2;
                _limitTop =   cameraSize.y / 2;
                _limitBtm =  -cameraSize.y / 2;
            }
            _lowPassDir = new LowPassFilter(damping, 0);
        }
        public void Install(int index, int level)
        {
            _shipLevel = level;
            _shipPower = GameInformation.Instance.GetShipData(index).GetPower(level - 1);
        }
        public void InstallHealth(int lives)
        {
            currentHealth = maxHealth = lives;
        }
        public override void OnUpdateHP()
        {
            GamePlayManager.Instance.UIManager.UpdateLives((int)currentHealth);
            if(currentHealth == 0){
                SoundManager.Instance.PlaySFX("player_gothit");
                GamePlayManager.Instance.GameOver();
            } else {
                GamePlayManager.Instance.UIManager.PlayGotHit();
                SoundManager.Instance.PlaySFX("player_gothit");
            }
        }

        public virtual void OnEventTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.tagEnemy))
            {
                SetUpHitByEnemy();
            }
            if (other.CompareTag(Constants.tagItem) && other.gameObject.activeSelf)
            {
                other.GetComponent<ItemController>().OnCollidePlayer();
            }
        }
        public override void OnParticleTriggerEnter2D(ParticleCollider other)
        {
            other.OnCollidePlayer();
        }
        public void OnParticleTriggerEnterMagnet(ParticleCollider other)
        {
            other.OnCollideMagnet();
        }
        protected override void HandleDirection()
        {
            dir = GamePlayManager.Instance.DPad.GetDragDistance() / (Time.fixedDeltaTime);
            _lowPassDir.Input(dir.x != 0 ? dir.x / Mathf.Abs(dir.x) : 0);
            angle = -_lowPassDir.Output() / _lowPassDir.GetAlpha() * velAngle;
            float tmpAngle = angle;
            if (roundAngle > 0)
            {
                tmpAngle = Mathf.Round(tmpAngle / roundAngle) * roundAngle;
            }
            //tmpAngle = Mathf.Clamp(tmpAngle,-maxAngle, maxAngle);
            _tfModel.rotation = Quaternion.Euler(0, tmpAngle, 0);
        }
        public void SetUpHitByEnemy()
        {
            if (!_isImmortal)
            {
                TakeDamage(null, 1);
                _isImmortal = true;
                StartCoroutine(SetUpImmortalAfterHit());
            }
        }
        public void SetUpHitByEnemyBullet(BulletData bulletData)
        {
            if (!_isImmortal)
            {
                TakeDamage(null, 1);
                _isImmortal = true;
                StartCoroutine(SetUpImmortalAfterHit());
            }
            if (!bulletData.canPenetrated)
            {
                bulletData.ApplyEfect.callbackSelfDestruction?.Invoke();
            }
        }

        public IEnumerator SetUpImmortalAfterHit()
        {
            _isImmortal = true;
            yield return Yielder.Get(1);
            _isImmortal = false;
        }
    }
}