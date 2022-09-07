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
    }

    public void SetFillBar(int curTime, int prevStartTime, int interval){
        fillBarManager.SetRawValue(curTime - prevStartTime, interval);
        fillBarTimeTextManager.SetValue(interval);
    }
}
