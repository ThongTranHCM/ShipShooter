using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThongNguyen.PlayerController
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public Collider2D physicsCollider;
        Rigidbody2D rigidBody2d;
        public Collider2D onHitCollider;
        [SerializeField]
        protected Transform _tfModel;
        public float speed;
        public bool isNormalize;
        public bool limitInsideScreen;
        [SerializeField]
        private Vector3 _drawSize;
        protected float _limitRight, _limitLeft, _limitTop, _limitBtm;
        public float MaxHp
        {
            get { return maxHealth; }
            set {
                maxHealth = value;
            }
        }
        protected float maxHealth;
        public float CurrentHp
        {
            get { return currentHealth; }
            set {
                currentHealth = value;
                OnUpdateHP();
            }
        }
        protected float currentHealth;
        protected Vector2 dir;

        protected virtual void Start()
        {
            if (physicsCollider != null) rigidBody2d = physicsCollider.GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            HandleDirection();
            Move();
        }
        protected virtual void HandleDirection()
        {
            dir = Vector2.zero;
        }

        public virtual void OnUpdateHP()
        {
        }
        public virtual float GetSpeed()
        {
            return speed;
        }
        protected virtual void Move()
        {
            if (physicsCollider != null)
            {
                transform.position = physicsCollider.transform.position;
                physicsCollider.transform.localPosition = Vector2.zero;
            }
            if (rigidBody2d != null)
                rigidBody2d.velocity = Vector2.zero;
            if (isNormalize)
            {
                dir.Normalize();
            }
            Vector3 finalPos = transform.position + (Vector3)(GetSpeed() * dir * Time.fixedDeltaTime);
            if (limitInsideScreen)
            {
                finalPos.x = Mathf.Clamp(finalPos.x, _limitLeft + _drawSize.x/2, _limitRight - _drawSize.x / 2);
                finalPos.y = Mathf.Clamp(finalPos.y, _limitBtm + _drawSize.y / 2, _limitTop - _drawSize.y / 2);
            }
            transform.position = finalPos;
        }

        public virtual void OnParticleTriggerEnter2D(BulletParticleSystem other) { }
        public virtual void OnParticleTriggerEnter2D(ParticleCollider other) { }
        public virtual void SetUpHit(BulletData bulletData, Vector2 posOfHit) { }

        public virtual void TakeDamage(ApplyEffectData effectData, float damage)
        {
            CurrentHp -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
                OnDie();
            }
        }
        public virtual void OnTriggerEnterCollider(Collider2D _otherCollider) { }


        public virtual void OnDie()
        {
        }

        public void OnDrawGizmos()
        {
            Vector3 topLeft = transform.position - (Vector3)_drawSize / 2;
            Gizmos.DrawLine(topLeft, topLeft + new Vector3(_drawSize.x, 0, 0));
            Gizmos.DrawLine(topLeft + _drawSize, topLeft + new Vector3(_drawSize.x, 0, 0));
            Gizmos.DrawLine(topLeft + _drawSize, topLeft + new Vector3(0, _drawSize.y, 0));
            Gizmos.DrawLine(topLeft, topLeft + new Vector3(0, _drawSize.y, 0));
        }
    }
}
