using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionPanelManager : MonoBehaviour
{
    private TimeChestManager.Mission mission;
    [SerializeField]
    private FillBarManager fillBarManager;
    [SerializeField]
    private FillBarBaseTextManager fillBarTextManager;
    [SerializeField]
    private GameObject claimButton;
    [SerializeField]
    private GameObject goToButton;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private TextMeshProUGUI reward;

    public void SetMission(TimeChestManager.Mission TargetMission){
        description.text = TargetMission.Description;
        reward.text = ((float)TargetMission.Reward / 3600).ToString();
        claimButton.gameObject.SetActive(TargetMission.IsFinish());
        goToButton.gameObject.SetActive(!TargetMission.IsFinish());
        if( mission != null ){
            if (mission.CurProgress != TargetMission.CurProgress || mission.Requirement != TargetMission.Requirement){
                fillBarManager.UpdateRawValue(TargetMission.CurProgress, TargetMission.Requirement);
            }
        } else {
            fillBarManager.UpdateRawValue(TargetMission.CurProgress, TargetMission.Requirement);
        }
        fillBarTextManager.SetValue(TargetMission.Requirement);
        mission = TargetMission;
    }

    public void CompleteMission(){
        TimeChestManager.Instance.CompleteMission(mission);
    }
}
