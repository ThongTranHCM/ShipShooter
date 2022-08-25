/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public interface ScrollSnapDelegate
    {
        bool ShouldMoveToPage(int pageIndex);
    }

    [RequireComponent(typeof(ScrollRectNested))]
    [AddComponentMenu("Layout/Extensions/Scroll Snap")]
    public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        protected Transform _screensContainer;

        private int _screens = 1;
        public System.Action<int> OnScreenChangedByDragging;
        public System.Action<int> OnScreenChangedByButton;
        public System.Action<int> didMoveToScreen;
        public ScrollSnapDelegate del;

        protected bool _fastSwipeTimer = false;
        protected int _fastSwipeCounter = 0;
        protected int _fastSwipeTarget = 30;

        private System.Collections.Generic.List<Vector3> _positions;
        protected ScrollRect _scrollRect;
        private Vector3 _lerpTarget;
        private bool _lerp;
        private bool _isInited;

        [Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
        public GameObject Pagination;
        [Tooltip("Transition speed between pages. (optional)")]
        [SerializeField]
        private float _transitionSpeed = 7.5f;
        [SerializeField]
        protected int _fastSwipeThreshold = 100;

        private bool _startDrag = true;
        protected Vector3 _startPosition = new Vector3();

        [Tooltip("The currently active page")]
        [SerializeField]
        private int _currentScreen;
        private int _targetScreen;

        [Tooltip("The screen / page to start the control on")]
        [SerializeField]
        private int _startingScreen = 1;

        [SerializeField]
        private int _pageStep;
        protected virtual float DefaultPageStep
        {
            get {
                if (!_isHorizon) { return _scrollRect.GetComponent<RectTransform>().rect.height; }
                else { return _scrollRect.GetComponent<RectTransform>().rect.width; }
            }
        }
        private bool _isHorizon = true;
        private bool IsHorizon { get { return _isHorizon; } }
        private bool IsVertical { get { return ! _isHorizon; } }

        protected virtual float ScrollRectNormalizePos
        {
            set { if (!_isHorizon)
                {
                    _scrollRect.verticalNormalizedPosition = value;
                }
                else
                {
                    _scrollRect.horizontalNormalizedPosition = value;
                }
            }
        }

        // Use this for initialization
        public void Init()
        {
            _isInited = true;
            _scrollRect = gameObject.GetComponent<ScrollRect>();

            if (_scrollRect.horizontalScrollbar || _scrollRect.verticalScrollbar)
            {
                Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
            }

            _screensContainer = _scrollRect.content;
            if (_pageStep == 0)
            {
                _pageStep = (int)DefaultPageStep;
            }
            DistributePages();

            _lerp = false;
            if (!_isInited)
            {
                _currentScreen = _startingScreen;
            }
            else
            {
                _currentScreen = Mathf.Clamp(_currentScreen, 0, _screens - 1);
            }

            ScrollRectNormalizePos = (float)(_currentScreen - 1) / (float)(_screens - 1);

            ChangeBulletsInfo(_currentScreen);

            OnScreenChangedByDragging?.Invoke(_currentScreen);
        }

        void Update()
        {
            if (_lerp)
            {
                _screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerpTarget, _transitionSpeed * Time.deltaTime);
                if (Vector3.Distance(_screensContainer.localPosition, _lerpTarget) < 0.01f)
                {
                    _lerp = false;
                    didMoveToScreen?.Invoke(_targetScreen);
                }

                //change the info bullets at the bottom of the screen. Just for visual effect
                /*if (Vector3.Distance(_screensContainer.localPosition, _lerpTarget) < 0f)
                {
                    ChangeBulletsInfo(CurrentScreen());
                }*/
            }

            if (_fastSwipeTimer)
            {
                _fastSwipeCounter++;
            }
        }


        //Function for switching screens with buttons
        public void NextScreen()
        {
            if (_currentScreen < _screens - 1)
            {
                _currentScreen++;
                _lerp = true;
                _targetScreen = _currentScreen;
                _lerpTarget = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching screens with buttons
        public void PreviousScreen()
        {
            if (_currentScreen > 0)
            {
                _currentScreen--;
                _lerp = true;
                _targetScreen = _currentScreen;
                _lerpTarget = _positions[_currentScreen];

                ChangeBulletsInfo(_currentScreen);
            }
        }

        //Function for switching to a specific screen
        public void GoToScreen(int screenIndex, bool animated = true)
        {
            if (screenIndex <= _screens && screenIndex >= 0)
            {
                if (animated)
                {
                    _lerp = true;

                    if (del != null)
                    {
                        if (!del.ShouldMoveToPage(screenIndex))
                        {
                            return;
                        }
                    }

                    _lerpTarget = _positions[screenIndex];
                    _targetScreen = screenIndex;

                    ChangeBulletsInfo(screenIndex);
                }
                else
                {
                    _lerpTarget = _positions[screenIndex];
                    _screensContainer.localPosition = _lerpTarget;
                    _currentScreen = screenIndex;

                    ChangeBulletsInfo(screenIndex);
                    didMoveToScreen?.Invoke(screenIndex);
                }
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        protected void NextScreenCommand()
        {
            if (_currentScreen < _screens - 1)
            {
                _lerp = true;
                _lerpTarget = _positions[_currentScreen + 1];
                _targetScreen = _currentScreen + 1;

                ChangeBulletsInfo(_currentScreen + 1);
                OnScreenChangedByDragging?.Invoke(_currentScreen + 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        protected void PrevScreenCommand()
        {
            if (_currentScreen > 0)
            {
                _lerp = true;
                _lerpTarget = _positions[_currentScreen - 1];
                _targetScreen = _currentScreen - 1;

                ChangeBulletsInfo(_currentScreen - 1);
                OnScreenChangedByDragging?.Invoke(_currentScreen - 1);
            }
        }

        //find the closest registered point to the releasing point
        private Vector3 FindClosestFrom(Vector3 start)
        {
            Vector3 closest = Vector3.zero;
            float distance = Mathf.Infinity;

            foreach (Vector3 position in _positions)
            {
                if (Vector3.Distance(start, position) < distance)
                {
                    distance = Vector3.Distance(start, position);
                    closest = position;
                }
            }
            return closest;
        }

        //returns the current screen that the is seeing
        public int CurrentScreen()
        {
            var pos = FindClosestFrom(_screensContainer.localPosition);
            return _currentScreen = GetPageforPosition(pos);
        }

        //changes the bullets on the bottom of the page - pagination
        private void ChangeBulletsInfo(int currentScreen)
        {
            if (Pagination)
                for (int i = 0; i < Pagination.transform.childCount; i++)
                {
                    Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (currentScreen == i)
                        ? true
                        : false;
                }
        }

        //used for changing between screen resolutions
        protected virtual void DistributePages()
        {
            if (_isHorizon)
            {
                DistributePageHorizontal(0);
            }
            else
            {
                DistributePageVertical(0);
            }

            _screens = _screensContainer.childCount;

            _positions = new System.Collections.Generic.List<Vector3>();

            if (_screens > 0)
            {
                for (int i = 0; i < _screens; ++i)
                {
                    ScrollRectNormalizePos = (float)i / (float)(_screens - 1);
                    _positions.Add(_screensContainer.localPosition);
                }
            }
        }

        private void DistributePageHorizontal(float offset)
        {
            Vector2 panelDimensions = GetComponent<RectTransform>().sizeDelta;
            float currentXPosition = 0;

            for (int i = 0; i < _screensContainer.childCount; i++)
            {
                RectTransform child = _screensContainer.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentXPosition = offset + i * _pageStep;
                child.SetWidth(_pageStep);
                child.anchoredPosition = new Vector2(currentXPosition - panelDimensions.x / 2, 0f + panelDimensions.y / 2);
            }

            _screensContainer.GetComponent<RectTransform>().SetWidth(currentXPosition + _pageStep + offset * -1);
        }   
        private void DistributePageVertical(float offset)
        {
            Vector2 panelDimensions = GetComponent<RectTransform>().sizeDelta;
            float currentYPosition = 0;
            for (int i = 0; i < _screensContainer.childCount; i++)
            {
                RectTransform child = _screensContainer.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentYPosition = offset + i * _pageStep;
                child.SetHeight(_pageStep);
                child.anchoredPosition = new Vector2(0f - panelDimensions.x / 2, currentYPosition + panelDimensions.y / 2);
            }
            _screensContainer.GetComponent<RectTransform>().SetHeight(currentYPosition + _pageStep + offset * -1);
        }

        int GetPageforPosition(Vector3 pos)
        {
            for (int i = 0; i < _positions.Count; i++)
            {
                if (_positions[i] == pos)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Add a new child to this Scroll Snap and recalculate it's children
        /// </summary>
        /// <param name="gObject">GameObject to add to the ScrollSnap</param>
        public void AddChild(GameObject gObject)
        {
            ScrollRectNormalizePos = 0;
            gObject.transform.SetParent(_screensContainer);
            DistributePages();
            ScrollRectNormalizePos = (float)(_currentScreen) / (_screens - 1);
        }

        /// <summary>
        /// Remove a new child to this Scroll Snap and recalculate it's children 
        /// *Note, this is an index address (0-x)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="childRemoved"></param>
        public void RemoveChild(int index, out GameObject childRemoved)
        {
            childRemoved = null;
            if (index < 0 || index > _screensContainer.childCount)
            {
                return;
            }
            ScrollRectNormalizePos = 0;
            var children = _screensContainer.transform;
            int i = 0;
            foreach (Transform child in children)
            {
                if (i == index)
                {
                    child.SetParent(null);
                    childRemoved = child.gameObject;
                    break;
                }
                i++;
            }
            DistributePages();
            if (_currentScreen > _screens - 1)
            {
                _currentScreen = _screens - 1;
            }

            ScrollRectNormalizePos = (float)(_currentScreen) / (_screens - 1);
        }

        #region Interfaces
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = _screensContainer.localPosition;
            _fastSwipeCounter = 0;
            _fastSwipeTimer = true;
            _currentScreen = CurrentScreen();
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (_startDrag)
            {
                return;
            }
            _startDrag = true;
            _fastSwipeTimer = false;
            Vector3 lLerp_target;
            if (!CheckFastSwipe())
            {
                _lerp = true;
                int oldScreen = _currentScreen;
                lLerp_target = _screensContainer.localPosition + (Vector3)(_scrollRect.velocity) * Time.deltaTime;
                lLerp_target = FindClosestFrom(lLerp_target);
                _scrollRect.velocity = Vector3.zero;
                int lcurrentScreen = GetPageforPosition(lLerp_target);

                if (del != null)
                {
                    if (!del.ShouldMoveToPage(lcurrentScreen))
                    {
                        return;
                    }
                }

                _currentScreen = lcurrentScreen;
                _lerpTarget = lLerp_target;
                if (oldScreen != _currentScreen)
                    if (OnScreenChangedByDragging != null)
                        OnScreenChangedByDragging(_currentScreen);
            }
        }

        protected virtual bool CheckFastSwipe()
        {
            bool isFastSwipe = false;
            if (_fastSwipeCounter <= _fastSwipeTarget)
            {
                float distance;
                if (_isHorizon)
                {
                    distance = _startPosition.y - _screensContainer.localPosition.y;
                }
                else
                {
                    distance = _startPosition.x - _screensContainer.localPosition.x;
                }
                isFastSwipe = Math.Abs(distance) > _fastSwipeThreshold;
                if (isFastSwipe)
                {
                    if (distance > 0)
                    {
                        NextScreenCommand();
                    }
                    else
                    {
                        PrevScreenCommand();
                    }
                }
            }
            return isFastSwipe;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _lerp = false;
            if (_isInited && _startDrag)
            {
                OnBeginDrag(eventData);
                _startDrag = false;
            }
        }
        #endregion
    }
}