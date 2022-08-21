using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryModePanelController : PanelController
{
    [SerializeField]
    private Button _btnPlay;
    [SerializeField]
    private Button _btnChest;
    [SerializeField]
    private Button _btnShop;
    [SerializeField]
    private Button _btnEquipment;

    public bool isFirstInit = true;
    public void OnButtonPlayClicked()
    {
        DataManager.Save();
        DataManager.Instance.selectedLevelIndex = DataManager.Instance.LastLevelWin + 1;
        SceneLoader.LoadLevel(Constants.SCENENAME_GamePlay);
    }

    public void Awake()
    {
        isFirstInit = false;
    }
}
