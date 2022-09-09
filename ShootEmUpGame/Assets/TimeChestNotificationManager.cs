using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeChestNotificationManager : MonoBehaviour
{
    [SerializeField]
    private GameObject claimNotification;
    [SerializeField]
    private GameObject timerNotification;
    [SerializeField]
    private TextMeshProUGUI timerText;
    // Update is called once per frame

    void Update()
    {
        if(TimeChestManager.Instance != null ){
            if(TimeChestManager.Instance.CheckClaimNotification()){
                if(timerNotification.activeSelf){
                    timerNotification.SetActive(false);
                }
                if(!claimNotification.activeSelf){
                    LeanTween.cancel(claimNotification);
                    LTSeq seq = LeanTween.sequence();
                    seq.append(() => claimNotification.SetActive(true));
                    seq.append(LeanTween.scale(claimNotification, Vector3.zero, 0.0f));
                    seq.append(LeanTween.scale(claimNotification, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack));
                }
            }
             else {
                if(claimNotification.activeSelf){
                    claimNotification.SetActive(false);
                }
                if(!timerNotification.activeSelf){
                    LeanTween.cancel(timerNotification);
                    LTSeq seq = LeanTween.sequence();
                    seq.append(() => timerNotification.SetActive(true));
                    seq.append(LeanTween.scale(timerNotification, Vector3.zero, 0.0f));
                    seq.append(LeanTween.scale(timerNotification, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack));
                }
                timerText.text =  TimeChestManager.Instance.GetTimeCountDown();
            }
        }
    }
}
