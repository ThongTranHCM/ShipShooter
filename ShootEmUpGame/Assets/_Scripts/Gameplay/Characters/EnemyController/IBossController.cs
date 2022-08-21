using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;

namespace ThongNguyen.PlayerController
{
    public class IBossController : TopDownCharacterController
    {
        [SerializeField]
        private GameObject _hpBarPrefab;
        [SerializeField]
        private Collider2D _enemyCollider;
        [SerializeField]
        private CharacterHealthbar _healthBar;
        protected override void Start()
        {
            base.Start();
            Install(25000);
        }

        public virtual void Install(int _hpLevel)
        {
            gameObject.SetActive(true);
            CurrentHp = MaxHp = _hpLevel;
            if (_healthBar == null)
            {
                var hpBar = PoolManager.Pools[Constants.poolMonster].Spawn(_hpBarPrefab);
                _healthBar = hpBar.GetComponent<CharacterHealthbar>();
                _healthBar.Install(transform, _hpLevel, _hpLevel);
            }
        }

        public override void OnUpdateHP()
        {
            if (_healthBar != null) _healthBar.SetCurrentHealth(currentHealth);
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
            if (dir.magnitude < 0.8f)
                dir = Vector2.zero;
            else
            {
                dir = dir.normalized;
                /*if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    animator.transform.rotation = Quaternion.Euler(0, tmpAngle, 0);
                    if (dir.x < 0)
                        animator.SetInteger("Direction", 3);
                    else
                        animator.SetInteger("Direction", 2);
                }
                else
                {
                    if (dir.y > 0)
                        animator.SetInteger("Direction", 1);
                    else
                        animator.SetInteger("Direction", 0);
                }*/
            }
        }

        public override void OnDie()
        {
            if (_healthBar != null)
            {
                PoolManager.Pools[Constants.poolMonster].Despawn(_healthBar.transform);
                _healthBar = null;
            }
            gameObject.SetActive(false);
            PoolManager.Pools[Constants.poolMonster].Despawn(transform);
        }
    }
}
