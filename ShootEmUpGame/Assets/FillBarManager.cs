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

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        changeLowPass.Input(0);
        float progress = Mathf.Clamp(valueLowPass.Output(), 0, 1);
        
        fillImage.color = borderImage.color = (value != 1) ? baseColor : fullColor;
        fillImage.color = borderImage.color += highlight * Color.white * Mathf.Clamp(changeLowPass.Output() / changeLowPass.GetAlpha(),0,0.1f);
        */
    }

    private void UpdateFillBar(){
        float offset = borderRectTransform.rect.width * Mathf.Clamp(animatedValue, 0, 1);
        fillRectTransform.sizeDelta = new Vector2(offset, fillRectTransform.sizeDelta.y);
    }

    public void SetValue(float Value)
    {
        float newValue = Value;
        newValue = Mathf.Clamp(newValue, 0, 1);
        PlayAnimation(value, newValue, 0.5f);
        value = newValue;
    }
    public void SetValue(float current, float max)
    {
        float newValue = current/max;
        newValue = Mathf.Clamp(newValue, 0, 1);
        PlayAnimation(value, newValue, 0.5f);
        value = newValue;
    }

    public void AddValue(float Value)
    {
        float newValue = value + Value;
        newValue = Mathf.Clamp(newValue, 0, 1);
        PlayAnimation(value, newValue, 0.5f);
        value = newValue;
    }

    public void PlayAnimation(float a, float b, float duration){
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject,(float x) => {animatedValue = x; UpdateFillBar();},a,b,duration).setEase(LeanTweenType.easeOutBack);
        LeanTween.value(gameObject,(float x) => {fillImage.color = borderImage.color = (value != 1) ? baseColor : fullColor; fillImage.color = borderImage.color += highlight * Color.white * x;},1,0,duration).setEase(LeanTweenType.easeInOutSine);
    }
}
