using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathUtil;
using TMPro;

public class FillBarManager : MonoBehaviour
{
    [SerializeField]
    private float highlight;
    [SerializeField]
    private float value;
    [SerializeField]
    private Color baseColor;
    [SerializeField]
    private Color fullColor;
    [SerializeField]
    private GameObject fill;
    [SerializeField]
    private GameObject border;
    [SerializeField]
    private TextMeshProUGUI txtProgress;
    private RectTransform fillRectTransform;
    private RectTransform borderRectTransform;
    private Image fillImage;
    private Image borderImage;
    private float animatedValue;
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        fillRectTransform = fill.GetComponent<RectTransform>();
        borderRectTransform = border.GetComponent<RectTransform>();
        fillImage = fill.GetComponent<Image>();
        borderImage = border.GetComponent<Image>();
        fillImage.color = borderImage.color = baseColor;
        animatedValue = value;
        UpdateFillBar();
    }

    private void UpdateFillBar(){
        float offset = borderRectTransform.rect.width * Mathf.Clamp(animatedValue, 0, 1);
        fillRectTransform.sizeDelta = new Vector2(offset, fillRectTransform.sizeDelta.y);
    }

    public void SetValue(float Value)
    {
        value = Mathf.Clamp(Value, 0, 1);
        animatedValue = value;
        UpdateFillBar();
    }
    public void SetRawValue(float current, float max)
    {
        value = Mathf.Clamp(current / max, 0, 1);
        animatedValue = value;
        UpdateFillBar();
    }

    public void UpdateValue(float Value, float duration = 0.5f, float delay = 0.0f)
    {
        float newValue = Value;
        newValue = Mathf.Clamp(newValue, 0, 1);
        float oldValue = value;
        value = newValue;
        PlayAnimation(oldValue, newValue, duration, delay);
    }
    public void UpdateRawValue(float current, float max, float duration = 0.5f, float delay = 0.0f)
    {
        float newValue = current/max;
        newValue = Mathf.Clamp(newValue, 0, 1);
        float oldValue = value;
        value = newValue;
        PlayAnimation(oldValue, newValue, duration, delay);
    }

    public void AddValue(float Value, float duration = 0.5f, float delay = 0.0f)
    {
        float newValue = value + Value;
        newValue = Mathf.Clamp(newValue, 0, 1);
        float oldValue = value;
        value = newValue;
        PlayAnimation(oldValue, newValue, duration, delay);
    }

    public void PlayAnimation(float a, float b, float duration, float delay){
        LeanTween.cancel(gameObject);
        LTSeq seq = LeanTween.sequence();
        seq.append(delay);
        seq.append( () => {
            LeanTween.value(gameObject,(float x) => {animatedValue = x; UpdateFillBar();},a,b,duration).setEase(LeanTweenType.easeOutBack);
            LeanTween.value(gameObject,(float x) => {fillImage.color = borderImage.color = (value != 1) ? baseColor : fullColor; fillImage.color = borderImage.color += Color.white * x;},1,0,duration).setEase(LeanTweenType.easeInSine);
        });
    }
}
