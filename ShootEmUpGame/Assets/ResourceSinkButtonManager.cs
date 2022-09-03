using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSinkButtonManager : MonoBehaviour
{
    [SerializeField]
    protected string resourceId;
    [SerializeField]
    protected int resourceCost;
    [SerializeField]
    protected ResourceTextManager costText;
    protected bool lastSufficentCheck = false;

    void Awake(){
        SetValue(resourceId, resourceCost);
    }

    public virtual void SetValue(string Id, int Cost){
        resourceId = Id;
        resourceCost = Cost;
        costText.SetValue(Id, Cost.ToString());
    } 
}
