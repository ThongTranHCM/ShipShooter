using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "onHitData_0", menuName = "Shooter/OnHitData")]
public class DOOnHitData : ScriptableObject
{
    public float totalDamageToTrigger;
    public GameObject onHitObject;
    public ApplyEffectData effectData;

    public DOOnHitData() { }
    public DOOnHitData(DOOnHitData other)
    {
        Init(other);
    }
    public void Init(DOOnHitData other)
    {
        totalDamageToTrigger = other.totalDamageToTrigger;
        onHitObject = other.onHitObject;
    }
}