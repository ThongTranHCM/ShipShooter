using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThongNguyen.PlayerController;

public class IParticleSystem : MonoBehaviour
{
    public ParticleSystem myParticleSystem;
    public int particleAmount;
    /// <summary>
    /// Mảng tạm chứa danh sách các collider tạm để kiểm tra
    /// </summary>
    [SerializeField]
    private Collider2D[] arrayCollider;
    /// <summary>
    /// Mảng tạm chứa các hạt particle tạm dùng để kiểm tra
    /// </summary>
    protected ParticleSystem.Particle[] _arrayParticle;

    /// <summary>
    /// Danh sách các hạt Particle Trigger Collider
    /// </summary>
    public Dictionary<uint, ParticleCollider> hashParticleCollider;

    /// <summary>
    /// Pool chứa các hạt trigger collider
    /// </summary>
    public Stack<ParticleCollider> poolParticleCollider;
    protected List<uint> _listParticleToDestroy;
    [SerializeField]
    private float _colliderRadius;

    /// <summary>
    /// Danh sách tạm chứa các Collider cần huỷ
    /// </summary>
    private Stack<ParticleCollider> stackColliderToDestroy;
    public bool isInstalled;
    private bool flag = false;
    #region UNITY FUNCTIONS
    public void Start()
    {
        _arrayParticle = new ParticleSystem.Particle[30];
        _listParticleToDestroy = new List<uint>();
        if (_colliderRadius == 0)
        {
            _colliderRadius = 1;
        }
    }

    public void OnDestroy()
    {
        if (poolParticleCollider != null)
        {
            while (poolParticleCollider.Count > 0)
            {
                var _tmpPoolParticleCollider = poolParticleCollider.Pop();
                _tmpPoolParticleCollider = null;
            }
            poolParticleCollider.Clear();
            poolParticleCollider = null;
        }
        if (hashParticleCollider != null)
        {
            hashParticleCollider.Clear();
            hashParticleCollider = null;
        }
        if (_listParticleToDestroy != null)
        {
            _listParticleToDestroy.Clear();
            _listParticleToDestroy = null;
        }
        if (_arrayParticle != null)
        {
            _arrayParticle = null;
        }
        if (arrayCollider != null)
        {
            arrayCollider = null;
        }
        if (stackColliderToDestroy != null)
        {
            stackColliderToDestroy.Clear();
            stackColliderToDestroy = null;
        }
    }

    public void Update()
    {
        particleAmount = myParticleSystem.GetParticles(_arrayParticle);

        for (int i = 0; i < particleAmount; i++)
        {
            OnUpdateParticle(i);
        }
    }

    public void FixedUpdate()
    {
        UpdateAllColliders();
        CleanExpiredColliders();
    }

    public virtual void LateUpdate()
    {
        if (_listParticleToDestroy == null || hashParticleCollider == null || _listParticleToDestroy == null
            || _arrayParticle == null)
        {
            return;
        }
        if (_listParticleToDestroy.Count == 0)
        {
            return;
        }

        if (myParticleSystem.particleCount == 0)
        {
            return;
        }

        // Huỷ các hạt particle nằm trong danh sách cần huỷ
        int particleAmount = myParticleSystem.GetParticles(_arrayParticle);

        for (int i = 0; i < particleAmount; i++)
        {
            var particle = _arrayParticle[i];
            if (_listParticleToDestroy.Contains(particle.randomSeed))
            {
                particle.remainingLifetime = 0;
                _arrayParticle[i] = particle;
            }
        }

        myParticleSystem.SetParticles(_arrayParticle, particleAmount);
        _listParticleToDestroy.Clear();
    }
    #endregion
    public virtual void OnParticleCollide(ParticleSystem.Particle particle, Collider2D collider)
    {
        if (collider != null)
        {
            TopDownCharacterController character = collider.transform.GetComponent<TopDownCharacterController>();
            if (character != null)
            {
                //character.OnParticleTriggerEnter2D(this);
            }
        }
    }
    public void Emit()
    {
        myParticleSystem.Emit(1);
    }

    public void StartEmit()
    {
        if (gameObject.activeSelf)
        {
            myParticleSystem.Play();
        }
    }
    public void StopEmit(System.Action callback, bool afterNoParticle)
    {
        if (gameObject.activeSelf)
        {
            myParticleSystem.Stop();
            if (afterNoParticle)
            {
                StartCoroutine(WaitUntilNoParticle(callback));
            }
            else
            {
                callback?.Invoke();
            }
        }
    }
    public IEnumerator WaitUntilNoParticle(System.Action callback)
    {
        while (myParticleSystem.particleCount > 0)
        {
            yield return Yielder.Get(1);
        }
        callback?.Invoke();
    }
    public void Install()
    {
        if (!isInstalled)
        {
            hashParticleCollider = new Dictionary<uint, ParticleCollider>(100);
            poolParticleCollider = new Stack<ParticleCollider>(100);
            _arrayParticle = new ParticleSystem.Particle[100];
            arrayCollider = new Collider2D[40];
            stackColliderToDestroy = new Stack<ParticleCollider>(100);

            if (myParticleSystem != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    poolParticleCollider.Push(new ParticleCollider(this));
                }
            }
            isInstalled = true;
        }
    }
    public void UpdateAllColliders()
    {
        if (myParticleSystem == null || hashParticleCollider == null || _arrayParticle == null || arrayCollider == null)
        {
            return;
        }
        if (myParticleSystem.particleCount == 0
            && hashParticleCollider.Count == 0)
        {
            return;
        }
        flag = !flag;
        particleAmount = myParticleSystem.GetParticles(_arrayParticle);
        for (int i = 0; i < particleAmount; i++)
        {
            var particle = _arrayParticle[i];
            var key = particle.randomSeed;
            if (!hashParticleCollider.ContainsKey(key))
            {
                hashParticleCollider[key] = CreateParticleCollider();
            }
            hashParticleCollider[key].CheckTrigger(particle, flag, arrayCollider);
        }
    }

    public virtual void OnUpdateParticle(int i)
    {
        ParticleSystem.Particle particle = _arrayParticle[i];
        int count = Physics2D.OverlapCircleNonAlloc((Vector2)particle.position, _colliderRadius, arrayCollider);
        if (count > 0)
        {
            for (int j = 0; j < count; j++)
            {
                OnParticleCollide(particle, arrayCollider[j]);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------
    /// <summary>
    /// Xoá các Particle Collider mà Particle đã bị huỷ
    /// </summary>
    //----------------------------------------------------------------------------------------------------
    public void CleanExpiredColliders()
    {
        if (hashParticleCollider == null)
        {
            return;
        }
        if (hashParticleCollider.Count == particleAmount)
        {
            return;
        }

        var enumerator = hashParticleCollider.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = enumerator.Current;

            if (item.Value.flag != flag)
            {
                stackColliderToDestroy.Push(item.Value);
            }
        }
        while (stackColliderToDestroy.Count > 0)
        {
            DestroyOneParticleCollider(stackColliderToDestroy.Pop());
        }
    }
    public virtual void SelfDestruction(ParticleSystem.Particle particle)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        particle.remainingLifetime = 0;
    }

    public virtual ParticleCollider CreateParticleCollider()
    {
        ParticleCollider particleTriggerCollider = null;
        if (poolParticleCollider == null || poolParticleCollider.Count == 0)
        {
            return null;
        }
        particleTriggerCollider = poolParticleCollider.Pop();
        particleTriggerCollider.StartReset();
        if (particleTriggerCollider == null)
        {
            particleTriggerCollider = new ParticleCollider(this);
        }
        return particleTriggerCollider;
    }

    public virtual void OnParticleCollideEnemy(ParticleCollider particleCollider, IEnemyController enemy) { }
    public virtual void OnParticleCollidePlayer(ParticleCollider particleCollider) { }
    public virtual void OnParticleCollideMagnet(ParticleCollider particleCollider) { }

    public virtual void DestroyOneParticleCollider(ParticleCollider particleCollider)
    {
        poolParticleCollider.Push(particleCollider);
        hashParticleCollider.Remove(particleCollider.particle.randomSeed);
    }

    public void DestroyParticle(ParticleCollider collider)
    {
        _listParticleToDestroy.Add(collider.particle.randomSeed);
        collider.waitToDestroy = true;
    }

    public virtual void SetDestroyParticle(ParticleSystem.Particle particle)
    {
        particle.remainingLifetime = 0;
    }
}
