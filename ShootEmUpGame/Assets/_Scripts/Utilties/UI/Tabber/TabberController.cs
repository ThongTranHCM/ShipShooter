using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class TabberController : PanelController
{
    public event Action<int> OnTabIndexActived;
    public event Action<int> OnTabIndexDeactived;

    public TabberButton[] tabButtons;
    public PanelController[] tabItems;
    public ScrollRect scrollRect;
    public GameObject preventTouchMask;
    public UnityEngine.UI.Extensions.ScrollSnap scrollSnap;

    public RectTransform tabHighlighter;
    private float[] _tabHighlighterPositions;

    public float transitionSpeed = 7.5f;
    public int startTab = 0;

    public UnityEvent<int> didMoveToTabEvent;

    RectTransform myRectransform
    {
        get
        {
            if (_myRectransform == null)
                _myRectransform = transform as RectTransform;
            return _myRectransform;
        }
    }
    private RectTransform _myRectransform;
    private LayoutElement[] _tabButtonsLayoutElements;
    private float[] _tabButtonWidth;
    [NonSerialized]
    public int currentTab;
    private int _lastTab;
    private bool _isInited;

    [SerializeField]
    private float _highlightedPreferedTabWidth = 400f;
    [SerializeField]
    private float _normalPreferedTabWidth = 200f;
    private Vector3[] _tabPositions;

    #region Pass Appear Event
    public override void PanelWillAppear()
    {
        base.PanelWillAppear();
        if (_isInited)
            tabItems[currentTab].PanelWillAppear();
    }

    public override void PanelDidAppear()
    {
        base.PanelDidAppear();
        if (_isInited)
            tabItems[currentTab].PanelDidAppear();
    }

    public override void PanelWillDisappear()
    {
        base.PanelWillDisappear();
        if (_isInited)
        {
            tabItems[currentTab].PanelWillDisappear();
        }
    }

    public override void PanelDidDisappear()
    {
        base.PanelDidDisappear();
        if (_isInited)
            tabItems[currentTab].PanelDidDisappear();
    }

    #endregion

    #region Init
    private void Start()
    {
        if (!_isInited)
            StartCoroutine(Init());
    }

    // Must be call after start function so the Rectransform of all element is inited
    public IEnumerator Init()
    {
        if (_isInited)
            yield break;
        float totalWidth = myRectransform.rect.width;
        float realNormalWidth = 0;
        float realHighlightedWidth = 0;

        realNormalWidth = totalWidth / (_highlightedPreferedTabWidth / _normalPreferedTabWidth + (tabButtons.Length - 1));
        realHighlightedWidth = realNormalWidth * _highlightedPreferedTabWidth / _normalPreferedTabWidth;
        _tabHighlighterPositions = new float[tabButtons.Length];
        for (int i = 0; i < tabButtons.Length; i++)
        {
            _tabHighlighterPositions[i] = realNormalWidth * i;
        }
        tabHighlighter.SetWidth(realHighlightedWidth);

        _tabButtonsLayoutElements = new LayoutElement[tabButtons.Length];
        _tabButtonWidth = new float[tabButtons.Length];
        for (int i = 0; i < tabButtons.Length; i++)
        {
            _tabButtonsLayoutElements[i] = tabButtons[i].GetComponent<LayoutElement>();
            if (i == 0)
            {
                _tabButtonWidth[i] = _highlightedPreferedTabWidth;
                _tabButtonsLayoutElements[i].preferredWidth = _tabButtonWidth[i];
            }
            else
            {
                _tabButtonWidth[i] = _normalPreferedTabWidth;
                _tabButtonsLayoutElements[i].preferredWidth = _tabButtonWidth[i];
            }
        }

        yield return null;
        scrollSnap.Init();
        scrollSnap.OnScreenChangedByDragging += (a => SelectTab(a));
        SelectTab(startTab, true);
        yield return null;

        _isInited = true;
    }
    #endregion

    #region Actions/Events
    void OnScreenChangedByDragging(int targetScreen)
    {
        Debug.LogError("OnScreenChange By Dragging");
        SelectTab(targetScreen);
    }
    public void OnScrollRectValueChange(Vector2 position)
    {
        try
        {
            float x = _tabHighlighterPositions[_tabHighlighterPositions.Length - 1] * position.x;
            float y = tabHighlighter.anchoredPosition.y;

            Vector2 highlighterPosition = new Vector2(x, y);
            tabHighlighter.anchoredPosition = highlighterPosition;
        }
        catch (Exception e)
        {

        }
    }

    public void OnTabButtonClicked(Button button)
    {
        int buttonIndex = 0;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (button.gameObject.Equals(tabButtons[i].gameObject))
            {
                buttonIndex = i;
            }
        }
        if (currentTab != buttonIndex)
            SelectTab(buttonIndex);
    }
    #endregion

    #region Change Tab 
    public void SelectTab(int index, bool isFirstLoaded = false)
    {
        if (!isFirstLoaded)
        {
            //AudioManager.PlaySound(DataAudioList.Instance.listAudioInUi.switchTab);
        }
        _lastTab = currentTab;
        currentTab = index;

        scrollSnap.GoToScreen(index, !isFirstLoaded);
        StartCoroutine(InvokePanelAppearMethod(_lastTab, currentTab, isFirstLoaded));

        for (int i = 0; i < _tabButtonsLayoutElements.Length; i++)
        {
            if (currentTab == i)
            {
                _tabButtonWidth[i] = _highlightedPreferedTabWidth;
                tabButtons[i].SetTabState(TabberButton.TabButtonState.Highlighted);
            }
            else
            {
                _tabButtonWidth[i] = _normalPreferedTabWidth;
                tabButtons[i].SetTabState(TabberButton.TabButtonState.Normal);
            }
        }
        StartCoroutine(tweenTabButtons(isFirstLoaded));
    }

    IEnumerator InvokePanelAppearMethod(int lastTab, int currentTab, bool isFirstLoaded)
    {
        if (isFirstLoaded)
            yield return Yielder.Get(0.1f);

        bool movedToNewScreen = false;
        scrollSnap.didMoveToScreen = (int newScreen) =>
        {
            movedToNewScreen = true;
            scrollSnap.didMoveToScreen = null;
        };

        yield return null;
        try
        {
            if (!isFirstLoaded)
                tabItems[lastTab].PanelWillDisappear();
        }
        catch (Exception e) {}

        yield return Yielder.Get(0.2f);// Just to make sure ScrollRectOclusion did active the next page
        try
        {
            tabItems[currentTab].PanelWillAppear();
            OnTabIndexActived?.Invoke(currentTab);
        }
        catch (Exception e) {}

        yield return new WaitUntil(() => movedToNewScreen);

        try
        {
            if (!isFirstLoaded)
            {
                tabItems[lastTab].PanelDidDisappear();
                OnTabIndexDeactived?.Invoke(lastTab);
            }
            tabItems[currentTab].PanelDidAppear();
        }
        catch (Exception e) {}

        if (didMoveToTabEvent != null)
            didMoveToTabEvent.Invoke(currentTab);
    }

    IEnumerator tweenTabButtons(bool isFirstLoaded)
    {
        preventTouchMask.SetActive(true);

        while (true)
        {
            for (int i = 0; i < _tabButtonsLayoutElements.Length; i++)
            {
                _tabButtonsLayoutElements[i].preferredWidth = Mathf.Lerp(_tabButtonsLayoutElements[i].preferredWidth, _tabButtonWidth[i], transitionSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(_tabButtonsLayoutElements[currentTab].preferredWidth - _highlightedPreferedTabWidth) < 5)
            {
                _tabButtonsLayoutElements[currentTab].preferredWidth = _highlightedPreferedTabWidth;
                break;
            }
            yield return null;
        }

        preventTouchMask.SetActive(false);
    }
    #endregion
}
