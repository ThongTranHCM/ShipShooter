using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;

public class GoldParticleSystem : IParticleSystem
{
    public ParticleSystem myHomingParticleSystem;
    [SerializeField]
    private float _followingTime;
    [SerializeField]
    private float _magnetSpeed;
    [SerializeField]
    private float _collectSize;
    protected ParticleSystem.Particle[] _homingParticle;

    public override void OnParticleCollidePlayer(ParticleCollider particleCollider)
    {
        GamePlayManager.Instance.Collection.AddGold(10);
        DestroyParticle(particleCollider);
    }
    public override void OnParticleCollideMagnet(ParticleCollider particleCollider)
    {
        EmitHomingParticle(particleCollider);
        DestroyParticle(particleCollider);
    }
    public void EmitHomingParticle(ParticleCollider particleCollider)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.startColor = particleCollider.particle.startColor;
        emitParams.velocity = particleCollider.particle.velocity;
        emitParams.position = particleCollider.particle.position;
        //emitParams.rotation3D = particleCollider.particle.rotation3D;
        emitParams.startLifetime = _followingTime;
        myHomingParticleSystem.Emit(emitParams, 1);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (_homingParticle == null) _homingParticle = new ParticleSystem.Particle[30];
        int particleAmount = myHomingParticleSystem.GetParticles(_homingParticle);
        for (int i = 0; i < particleAmount; i++)
        {
            var particle = _homingParticle[i];
            Vector3 distance = IShipController.Instance.transform.position - particle.position;
            particle.velocity = (Vector3.RotateTowards(particle.velocity, distance, 12, 12)).normalized * _magnetSpeed;
            if (distance.magnitude < _collectSize)
            {
                GamePlayManager.Instance.Collection.AddGold(10);
                particle.remainingLifetime = 0;
            }
            _homingParticle[i] = particle;
        }
        myHomingParticleSystem.SetParticles(_homingParticle, particleAmount);
    }
}
