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
    private FillBarTextManager txtProgress;
    [SerializeField]
    private TextMeshProUGUI _txtUI;
    private RectTransform fillRectTransform = null;
    private RectTransform borderRectTransform = null;
    private Image fillImage = null;
    private Image borderImage = null;
    private float animatedValue;
    private bool didInit = false;
    
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
        didInit = true;
        if(txtProgress != null){
            txtProgress.Install(this);
        }       
        SetFillBar(value);
    }

    private void SetFillBar(float x)
    {
        if (didInit){
            float offset = borderRectTransform.rect.width * Mathf.Clamp(x, 0, 1);
            fillRectTransform.sizeDelta = new Vector2(offset, fillRectTransform.sizeDelta.y);
            //fillRectTransform.anchorMax = new Vector2(x, fillRectTransform.anchorMax.y);
            fillImage.color = borderImage.color = (x < 1) ? baseColor : fullColor;
        } else {
            Init();
            SetFillBar(x);
        }
        
    }
    public void SetValue(float Value)
    {
        value = Value;
        animatedValue = value;
        //if (_txtUI != null)
        //{
        //    _txtUI.text = Value.ToString();
        //}
        SetFillBar(value);
    }
    public void SetRawValue(float current, float max)
    {
        value = current / max;
        //value = Mathf.Clamp(value, 0, 1);
        //if (_txtUI != null)
        //{
        //    _txtUI.text = current.ToString();
        //}
        animatedValue = value;
        SetFillBar(value);
    }

    public void UpdateValue(float Value, float duration = 0.5f, float delay = 0.0f)
    {
        float newValue = Value;
        float oldValue = value;
        value = newValue;
        PlayAnimation(oldValue, newValue, duration, delay);
    }
    public void UpdateRawValue(float current, float max, float duration = 0.5f, float delay = 0.0f)
    {
        float newValue = current/max;
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
            LeanTween.value(gameObject,(float x) => {SetFillBar(x);},a,b,duration).setEase(LeanTweenType.easeOutBack);
            LeanTween.value(gameObject,(float x) => {animatedValue = x;},a,b,duration).setEase(LeanTweenType.easeInSine);
            LeanTween.value(gameObject,(float x) => {fillImage.color = borderImage.color = (value < 1) ? baseColor : fullColor; fillImage.color = borderImage.color += Color.white * x;},1,0,duration).setEase(LeanTweenType.easeInSine);
        });
    }

    public float GetAnimatedValue(){
        return animatedValue;
    }

    public FillBarTextManager GetFillBarTextManager(){
        return txtProgress;
    }

    public class FillBarTextManager: MonoBehaviour{
        protected FillBarManager fillBarManager;
        [SerializeField]
        protected TextMeshProUGUI txtTMP;
        public void Install(FillBarManager FillBarManager){
            fillBarManager = FillBarManager;
        }
        public void FixedUpdate(){
            UpdateText();
        }
        public virtual void UpdateText(){ return; }
    }
}
