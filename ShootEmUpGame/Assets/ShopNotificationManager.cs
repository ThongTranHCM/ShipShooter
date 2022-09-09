using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNotificationManager : MonoBehaviour
{
    [SerializeField]
    private GameObject newNotification;
    void Update(){
        if(DailyOfferManager.Instance != null){
            if(DailyOfferManager.Instance.HasFinished()){
                if(!newNotification.activeSelf){
                    newNotification.SetActive(true);
                    LTSeq seq = LeanTween.sequence();
                    seq.append(LeanTween.scale(newNotification, Vector3.zero, 0.0f));
                    seq.append(LeanTween.scale(newNotification, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack));
                }
            } else {
                if(newNotification.activeSelf){
                    newNotification.SetActive(false);
                }
            }
        }
    }
}
