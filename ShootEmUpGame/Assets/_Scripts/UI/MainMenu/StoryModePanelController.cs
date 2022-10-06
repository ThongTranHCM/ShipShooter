using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryModePanelController : PanelController
{
    public bool isFirstInit = true;
    public void OnButtonPlayClicked()
    {
        DataManager.Instance.selectedMode = Constants.MODE_Story;
        DataManager.Instance.selectedLevelIndex = DataManager.Instance.LastLevelWin;
        int shipIndex = DataManager.Instance.selectedShipIndex;
        DataManager.Instance.SetLastShipIndex(shipIndex, Constants.MODE_Story);
        DataManager.Instance.selectedShipLevel = DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel;
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_GamePlay);
    }
    public void OnButtonEndlessPlayClicked()
    {
        DataManager.Instance.selectedMode = Constants.MODE_Endless;
        DataManager.Instance.selectedLevelIndex = 0; //Bronze Silver Diamond stuffs
        int shipIndex = DataManager.Instance.selectedShipIndex;
        DataManager.Instance.SetLastShipIndex(shipIndex, Constants.MODE_Endless);
        DataManager.Instance.selectedShipLevel = DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel;
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_GamePlay);
    }
    public void OnButtonChallengePlayClicked()
    {
        DataManager.Instance.selectedMode = Constants.MODE_Challenge;
        int shipIndex = DataManager.Instance.selectedShipIndex;
        DataManager.Instance.selectedLevelIndex = DataManager.Instance.GetLastChallengeIndex(shipIndex);
        DataManager.Instance.SetLastShipIndex(shipIndex, Constants.MODE_Challenge);
        DataManager.Instance.selectedShipLevel = DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel;
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_GamePlay);
    }

    public void Awake()
    {
        isFirstInit = false;
    }
}
