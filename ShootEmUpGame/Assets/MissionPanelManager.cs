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

    public void SetMission(TimeChestManager.Mission Mission){
        mission = Mission;
        description.text = mission.Description;
        reward.text = ((float)mission.Reward / 3600).ToString();
        claimButton.gameObject.SetActive(mission.IsFinish());
        goToButton.gameObject.SetActive(!mission.IsFinish());
        fillBarManager.SetRawValue(mission.CurProgress, mission.Requirement);
        fillBarTextManager.SetValue(mission.Requirement);
    }
}
