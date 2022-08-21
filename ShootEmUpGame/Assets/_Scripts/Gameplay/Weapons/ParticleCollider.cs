using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;

public class ParticleCollider
{
    public ParticleSystem.Particle particle;
    private IParticleSystem _iSystem;
    private List<Collider2D> _listCollider;
    [System.NonSerialized] public bool flag;
    public int collisionMask;
    public bool waitToDestroy;

    public void StartReset()
    {
        waitToDestroy = false;
        _listCollider.Clear();
    }
    public ParticleCollider(IParticleSystem targetSystem)
    {
        _iSystem = targetSystem;
        collisionMask = Physics2D.GetLayerCollisionMask(targetSystem.gameObject.layer);
        _listCollider = new List<Collider2D>(40);
        waitToDestroy = false;
    }
    public void CheckTrigger(ParticleSystem.Particle targetParticle, bool newFlag, Collider2D[] arrayCollider)
    {
        flag = newFlag;
        particle = targetParticle;

        if (Constants.IsOutOfSceneGamePlay((Vector2)particle.position, Vector2.one))
        {
            DestroyParticle();
            return;
        }

        int count = Physics2D.OverlapBoxNonAlloc((Vector2)particle.position, Vector2.one, 0, arrayCollider, collisionMask);

        // Xoá những collider đã va chạm trước đó nhưng không còn va chạm nữa (Collider Exit)
        int index = 0;
        while (index < _listCollider.Count)
        {
            bool contain = false;
            for (int i = 0; i < arrayCollider.Length; i++)
            {
                if (arrayCollider.Equals(_listCollider[index]))
                {
                    contain = true;
                    break;
                }
            }
            if (!contain)
            {
                _listCollider.RemoveAt(index);
                continue;
            }
            index++;
        }

        // Thêm những collider mới va chạm vào danh sách (Collider Enter)
        for (int i = 0; i < count; i++)
        {
            var collider = arrayCollider[i];
            if (!_listCollider.Contains(collider))
            {
                _listCollider.Add(collider);
                TopDownCharacterController character = collider.transform.GetComponent<TopDownCharacterController>();
                if (character != null)
                {
                    character.OnParticleTriggerEnter2D(this);
                }
                ColliderTriggerHelper colliderHelper = collider.transform.GetComponent<ColliderTriggerHelper>();
                if (colliderHelper != null)
                {
                    colliderHelper.OnParticleTriggerEnter2D(this);
                }
            }
        }
    }

    public void OnCollideEnemy(IEnemyController enemy)
    {
        if (_iSystem != null)
        {
            _iSystem.OnParticleCollideEnemy(this, enemy);
        }
    }

    public void OnCollidePlayer()
    {
        if (_iSystem != null)
        {
            _iSystem.OnParticleCollidePlayer(this);
        }
    }
    public void OnCollideMagnet()
    {
        if (_iSystem != null)
        {
            _iSystem.OnParticleCollideMagnet(this);
        }
    }

    public void DestroyParticle()
    {
        if (_iSystem != null)
        {
            _iSystem.DestroyParticle(this);
        }
    }
}