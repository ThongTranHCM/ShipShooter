using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfferLidManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Claimed;
    [SerializeField]
    private GameObject Lock;

    public void SetClaimed(){
        Claimed.SetActive(true);
        Lock.SetActive(false);
    }

    public void SetLock(){
        Claimed.SetActive(false);
        Lock.SetActive(true);
    }

    public void SetUnlock(){
        Claimed.SetActive(false);
        Lock.SetActive(false);
    }
}
