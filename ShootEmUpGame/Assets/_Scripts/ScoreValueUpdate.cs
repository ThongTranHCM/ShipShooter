using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathUtil;
using TMPro;

public class ScoreValueUpdate : MonoBehaviour
{
    
    [SerializeField]
    private float _damping;
    private float _value;
    private LowPassFilter _valueLowPass;
    private LowPassFilter _scoringLowPass;
    private bool _isUpdating;
    private RectTransform _scoreRectTransform;
    private TextMeshProUGUI _textMeshProGUI;
    [SerializeField]
    private float _highlight;
    private Color _baseColor;
    // Start is called before the first frame update
    void Start()
    {
        _value = 0;
        _valueLowPass = new LowPassFilter(_damping, _value);
        _scoringLowPass = new LowPassFilter(_damping, _value);
        _isUpdating = false;
        _scoreRectTransform = this.transform.GetComponent<RectTransform>();
        _textMeshProGUI = this.transform.GetComponent<TextMeshProUGUI>();
        _baseColor = _textMeshProGUI.color;
    }

    public void UpdateProgress(float fromValue, float toValue)
    {
        _value = fromValue;
        float increment = toValue - fromValue;
        _value += increment;
        _valueLowPass.Input(_value);
        _scoringLowPass.Input(increment > 0 ? 1 : 0);
        float lowPass = _valueLowPass.Output();
        _textMeshProGUI.text = Mathf.Ceil(lowPass).ToString();
        lowPass = _scoringLowPass.Output() / _scoringLowPass.GetAlpha();
        _scoreRectTransform.transform.localScale = Vector3.one * (1 + _highlight * lowPass);
        _textMeshProGUI.color = _baseColor + Color.white * _highlight * lowPass;
    }
}
