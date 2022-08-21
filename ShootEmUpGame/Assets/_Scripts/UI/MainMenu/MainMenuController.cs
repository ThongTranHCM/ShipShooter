using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public enum ScreenID
{
    Home,
    StoryMode
}

public class MainMenuController : PanelController
{
    private MainMenuController _instance;
    public MainMenuController Instance
    {
        get { return _instance; }
    }
    private static bool isFirstLoad = true;

    [Header("Path Game Over Screen")]
    public Transform canvasGameOverScreen;
    [SerializeField] string pathOfGameOverScreen_StoryMode;
    public bool canShowScene { get; set; }
    public IGameOverUIController gameOverScreen { get; set; }

    [Header("MainMenu Setting")]
    public Action onShowLevelUp;
    public Action onHideLevelUp;
    public Action onShowEquipment;
    public Action onHideEquipment;
    public Action onShowChallenge;
    public Action onHideChallenge;
    public Action onShowLevel;
    public Action onHideLevel;
    public Action onShowPage;
    public Action onHidePage;
    public static MainMenuController instance;
    public StoryModePanelController storyModePanel;

    [Space(5)]
    [SerializeField]
    private RawImage _imgAvatar;
    [SerializeField]
    private RectTransform _tfBtnBuyDiamond;
    [SerializeField]
    private RectTransform _tfBtnBuyCoin;

    [SerializeField]
    private Text _txtCoin;
    [SerializeField]
    private Text _txtDiamond;
    [SerializeField]
    private LevelBarController _levelBar;

    [SerializeField]
    private RectTransform _tfBottomBar;
    [SerializeField]
    private RectTransform _tfAddOns;
    [SerializeField]
    private RectTransform _tfChallenge;
    [SerializeField]
    private RectTransform _tfLevel;
    [SerializeField]
    private RectTransform _tfPage;

    [Space(5)]
    public GameObject lockInteractionMask;

    #region Panel Life Cycle
    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    void ShowQuit()
    {
        Debug.LogError("ShowQuit");
    }

    void Start()
    {
        StartCoroutine(DoActionOnStart());
        onShowEquipment = () => { _tfAddOns.gameObject.SetActive(true); };
        onHideEquipment = () => { _tfAddOns.gameObject.SetActive(false); };
        onShowChallenge = () => { _tfChallenge.gameObject.SetActive(true); };
        onHideChallenge = () => { _tfChallenge.gameObject.SetActive(false); };
        onShowLevel = () => { _tfLevel.gameObject.SetActive(true); };
        onHideLevel = () => { _tfLevel.gameObject.SetActive(false); };
        onShowPage = () => { _tfPage.gameObject.SetActive(true);};
        onHidePage = () => { _tfPage.gameObject.SetActive(false);};
    }
    IEnumerator DoActionOnStart()
    {
        if (isFirstLoad)
        {
            isFirstLoad = false;
        }
        else
        {
            yield return null;
        }
        yield return Yielder.Get(2);

        GameObject screenGameOverObject = null;
        if (DataManager.Instance.dataSubmitScoreStoryMode)
        {
            screenGameOverObject = Instantiate(Constants.GetAssest<GameObject>(pathOfGameOverScreen_StoryMode));
        }

        if (screenGameOverObject != null)
        {
            screenGameOverObject.transform.SetParent(canvasGameOverScreen, false);
            gameOverScreen = screenGameOverObject.GetComponent<IGameOverUIController>();
        }

        _levelBar.SetLevel(DataManager.Instance.playerData.level, DataManager.Instance.playerData.exp, false);
        _levelBar.onLevelUp = (level) =>
        {
            Debug.LogError("LevelUp");
        };

        // Update Cash text
        UpdateCashValue();

        yield return null;
        //StartCoroutine(tabBarPanelController.Init());
        // Update Avatar text ( Player name)
        RefreshAvatar();

        yield return new WaitUntil(() => !storyModePanel.isFirstInit);
        canShowScene = true;
        if (true)//DataManager.Instance.mainMenuDefaultScreen == MainMenuDefaultScene.Story)
        {
            StartCoroutine(GoToStoryMode());
        }
    }
    #endregion
    public void RefreshAvatar()
    {
        if (!string.IsNullOrEmpty(DataManager.Instance.playerData.avatarUrl))
        {
            //DataManager.Instance.playerData.avatarUrl
            Texture2D obj = null;
            if (obj != null)
                _imgAvatar.texture = obj;
        }

    }

    IEnumerator waitForAvatar = null;
    IEnumerator WaitForAvatar()
    {
        while (Social.localUser.image == null)
        {
            yield return null;
        }

        _imgAvatar.texture = Social.localUser.image;
        waitForAvatar = null;
    }

    public void UpdateCashValue()
    {
        _txtCoin.text = DataManager.Instance.playerData.Coin.ToString();
        _txtDiamond.text = DataManager.Instance.playerData.Diamond.ToString();
    }

    #region ShortCut to screen

    public void GoToScreen(ScreenID screenId)
    {
        switch (screenId)
        {
            case ScreenID.Home:
                StartCoroutine(GoToHomeIfNeeded());
                break;
        }
    }

    IEnumerator GoToHomeIfNeeded()
    {
        yield return Yielder.Get(0.5f);
        /*int index = 1;
        if (tabBarPanelController.currentTab != index)
        {
            tabBarPanelController.OnTabButtonClicked(tabBarPanelController.tabButtons[index].GetComponent<Button>());
            yield return Yielders.Get(0.5f);
        }*/
    }
    IEnumerator GoToStoryMode()
    {
        yield return StartCoroutine(GoToHomeIfNeeded());
        yield return null;
    }

    public void ShowEquipments()
    {
        onShowEquipment?.Invoke();
    }
    public void HideEquipments()
    {
        onHideEquipment?.Invoke();
    }
    public void ShowLevel(){
        onShowLevel?.Invoke();
    }
    public void HideLevel(){
        onShowLevel?.Invoke();
    }
    public void ShowChallenge(){
        onShowChallenge?.Invoke();
    }
    public void HideChallenge(){
        onHideChallenge?.Invoke();
    }
    public void ShowPage(){
        onShowPage?.Invoke();
    }
    public void HidePage(){
        onHidePage?.Invoke();
    }
    #endregion
    #region other

    public void LockInteraction()
    {
        lockInteractionMask.SetActive(true);
    }

    public void UnlockInteraction()
    {
        lockInteractionMask.SetActive(false);
    }
    #endregion
}