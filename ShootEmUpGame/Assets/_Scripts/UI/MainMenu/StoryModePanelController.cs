using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryModePanelController : PanelController
{
    public bool isFirstInit = true;
    public void OnButtonPlayClicked()
    {
        DataManager.Instance.selectedLevelIndex = DataManager.Instance.LastLevelWin + 1;
        int shipIndex = DataManager.Instance.selectedShipIndex;
        DataManager.Instance.LastShipIndex = shipIndex;
        DataManager.Instance.selectedShipLevel = DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel;
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_GamePlay);
    }

    public void Awake()
    {
        isFirstInit = false;
    }
}
