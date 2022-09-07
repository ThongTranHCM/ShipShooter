using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChestContentManager : MonoBehaviour
{
    private static TimeChestContentManager instance = null;
    public static TimeChestContentManager Instance{
        get { return instance; }
    }
    [SerializeField]
    private FillBarManager fillBarManager;
    [SerializeField]
    private FillBarTimeTextManager fillBarTimeTextManager;
    [SerializeField]
    private GameObject missionPrefab;
    [SerializeField]
    private TabberController tabberController;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }
    void Start(){
        TimeChestManager.Instance.InitContent();
    }

    void FixedUpdate(){
        TimeChestManager.Instance.UpdateCounter();
    }

    public void UpdateMissionPanel(List<TimeChestManager.Mission> missionList){
        for(int i = transform.childCount; i < missionList.Count; i++){
            GameObject missionPanel = Instantiate(missionPrefab, transform.position, Quaternion.identity, transform);
        }
        for(int i = 0; i < missionList.Count; i++){
            GameObject missionPanel = transform.GetChild(i).gameObject;
            missionPanel.GetComponent<MissionPanelManager>().SetMission(missionList[i]);
            missionPanel.SetActive(missionList[i].IsActive);
        }
        return;
    }

    public void UpdatePurchaseButton(){
        return;
    }

    public void UpdateFillBar(int curTime, int prevStartTime, int interval){
        fillBarManager.UpdateRawValue(curTime - prevStartTime, interval, 1.0f);
        fillBarTimeTextManager.SetValue(interval);
        LTSeq seq = LeanTween.sequence();
        seq.append(() => {
            LeanTween.scaleY(fillBarManager.gameObject, 1.15f, 0.25f).setEase(LeanTweenType.easeOutBack);
            LeanTween.scaleX(fillBarManager.gameObject, 1.1f, 0.25f).setEase(LeanTweenType.easeOutBack);
        });
        seq.append(0.85f);
        seq.append(() => {
            LeanTween.scaleY(fillBarManager.gameObject, 1 / 1.15f, 0.25f).setEase(LeanTweenType.easeOutBack);
            LeanTween.scaleX(fillBarManager.gameObject, 1 / 1.1f, 0.25f).setEase(LeanTweenType.easeOutBack);
        });
        
    }

    public void SetFillBar(int curTime, int prevStartTime, int interval){
        fillBarManager.SetRawValue(curTime - prevStartTime, interval);
        fillBarTimeTextManager.SetValue(interval);
    }

    public bool IsInTab(){
        return tabberController.currentTab == 1;
    }
}
