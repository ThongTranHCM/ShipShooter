using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;

public class BulletParticleSystem : IParticleSystem
{
    public BulletData bulletData;
    [SerializeField]
    private FindTargetController _findTargetController; // script điều khiến việc tìm target để dí hoặc nhắm bắn

    public void Orientate()
    {
        GameObject targetLocked = null;
        float zDegAngle = 0, zRadAngle = 0;
        if (_findTargetController != null)
        {
            targetLocked = _findTargetController.FindTarget();
        }
        if (targetLocked != null && targetLocked.activeSelf)
        {
            Vector3 dir = targetLocked.transform.position - transform.position;
            dir.Normalize();
            zRadAngle = Mathf.Atan2(dir.y, dir.x) - Mathf.PI / 2;
            zDegAngle = zRadAngle * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.Euler(0, 0, zDegAngle);
        ParticleSystem.MainModule mainModule = myParticleSystem.main;
        mainModule.startRotationZ = zRadAngle;
    }

    public override void OnParticleCollide(ParticleSystem.Particle particle, Collider2D collider)
    {
        if (collider != null)
        {
            TopDownCharacterController character = collider.transform.GetComponent<TopDownCharacterController>();
            if (character != null)
            {
                character.OnParticleTriggerEnter2D(this);
            }
        }
    }
    public virtual void Install(DOGunData gunData, bool shoot)
    {
        if (!isInstalled)
        {
            if (gunData != null)
            {
                if (gunData.numWavePerShot == 0)
                {
                    ParticleSystem.EmissionModule emissionModule = myParticleSystem.emission;
                    emissionModule.rateOverTime = gunData.attackSpeed;
                }
                else
                {
                    myParticleSystem.Stop();
                }
                Install(gunData.bulletData);
            }
            if (!shoot)
            {
                myParticleSystem.Stop();
            }
        }
    }
    public virtual void Install(BulletData otherBulletData)
    {
        if (!isInstalled)
        {
            if (bulletData != null)
            {
                bulletData.Init(otherBulletData);
                ParticleSystem.MainModule mainModule = myParticleSystem.main;
                mainModule.startSpeedMultiplier *= bulletData.speed;
                mainModule.startLifetimeMultiplier = bulletData.timeToMove;
                bulletData.ApplyEfect.callbackSelfDestruction += SelfDestruction;
                bulletData.ApplyEfect.callbackShowEffectHit = AddEffectHit;
                bulletData.ApplyEfect.callbackShowEffectCritical = AddEffectCritical;
            }
            Install();
        }
    }
    public override void SelfDestruction(ParticleSystem.Particle particle)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (bulletData != null && bulletData.ApplyEfect != null
            && bulletData.ApplyEfect.callbackShotMiss != null)
        {
            bulletData.ApplyEfect.callbackShotMiss?.Invoke();
        }
        particle.remainingLifetime = 0;
    }
    public override void DestroyOneParticleCollider(ParticleCollider particleCollider)
    {
        if (bulletData.ApplyEfect.callbackShotMiss != null)
        {
            bulletData.ApplyEfect.callbackShotMiss();
            bulletData.ApplyEfect.callbackShotMiss = null;
        }
        base.DestroyOneParticleCollider(particleCollider);
    }
    public override void OnParticleCollideEnemy(ParticleCollider particleCollider, IEnemyController enemy)
    {
        if (bulletData.ApplyEfect.damageSource >= ApplyEffectData.DamageSource.FromMonster)
        {
            return;
        }
        enemy.SetUpHit(bulletData, Vector2.one);
        if (!bulletData.canPenetrated)
        {
            DestroyParticle(particleCollider);
        }
    }
    public override void OnParticleCollidePlayer(ParticleCollider particleCollider)
    {
        if (bulletData.ApplyEfect.damageSource < ApplyEffectData.DamageSource.FromMonster)
        {
            return;
        }
        IShipController.Instance.SetUpHitByEnemyBullet(bulletData);
        if (!bulletData.canPenetrated)
        {
            DestroyParticle(particleCollider);
        }
    }
    public void SelfDestruction()
    {
        //DestroyParticle(particleCollider);
    }
    public void AddEffectHit(Transform tf, Vector2 posHit) { }
    public void AddEffectCritical(Transform tf, Vector2 posHit) { }
}
