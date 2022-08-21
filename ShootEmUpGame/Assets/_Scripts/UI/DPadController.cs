using UnityEngine;
using UnityEngine.UI;
using Lean.Touch;

public class DPadController : MonoBehaviour
{
    private float _screenToGameMultiplier;
    private Vector2 _lastTouchPosition;
    private LeanFinger _finger;
    void OnEnable()
    {
        _screenToGameMultiplier = Constants.SizeOfCamera().y / Screen.height;

        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }

    public void OnFingerDown(LeanFinger finger)
    {
        //Check Buttons.
        //Check if Player exist. Check if it can move.
        _lastTouchPosition = finger.ScreenPosition;
        if (_finger == null)
        {
            _finger = finger;
        }
    }
    public void OnFingerUp(LeanFinger finger)
    {
        _finger = null;
    }

    public Vector2 GetDragDistance()
    {
        if (_finger != null)
        {
            Vector2 delta = (_finger.ScreenPosition - _lastTouchPosition) * _screenToGameMultiplier;
            _lastTouchPosition = _finger.ScreenPosition;
            return delta;
        }
        return Vector2.zero;
    }
}