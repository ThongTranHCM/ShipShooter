using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTriggerHelper : MonoBehaviour
{
    public UnityEvent<Collider2D> ColliderEvent;
    public UnityEvent<ParticleCollider> TriggerEnterEvent;

    public void OnTriggerEnter2D(Collider2D other)
    {
        ColliderEvent?.Invoke(other);
    }

    public void OnParticleTriggerEnter2D(ParticleCollider other)
    {
        TriggerEnterEvent?.Invoke(other);
    }
}
