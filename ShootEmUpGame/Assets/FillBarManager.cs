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
    private LowPassFilter valueLowPass;
    private LowPassFilter changeLowPass;
    private RectTransform fillRectTransform;
    private RectTransform borderRectTransform;
    private Image fillImage;
    private Image borderImage;
    
    // Start is called before the first frame update
    void Start()
    {
        if (valueLowPass == null)
        {
            Init();
        }
    }

    public void Init()
    {
        valueLowPass = new LowPassFilter(0.1f, value);
        changeLowPass = new LowPassFilter(0.1f, 0);
        fillRectTransform = fill.GetComponent<RectTransform>();
        borderRectTransform = border.GetComponent<RectTransform>();
        fillImage = fill.GetComponent<Image>();
        borderImage = border.GetComponent<Image>();
        fillImage.color = borderImage.color = baseColor;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        valueLowPass.Input(value);
        changeLowPass.Input(0);
        float progress = Mathf.Clamp(valueLowPass.Output(), 0, 1);
        float offset = borderRectTransform.rect.width * progress;
        fillRectTransform.sizeDelta = new Vector2(offset, fillRectTransform.sizeDelta.y);
        fillImage.color = borderImage.color = (value != 1) ? baseColor : fullColor;
        fillImage.color = borderImage.color += Color.white * changeLowPass.Output() / changeLowPass.GetAlpha();
    }

    public void SetValue(float Value)
    {
        if (valueLowPass == null)
        {
            Init();
        }
        changeLowPass.Input(1);
        value = Value;
    }
    public void SetValue(float current, float max)
    {
        if (txtProgress != null)
        {
            txtProgress.text = "" + current + "/" + max;
        }
        if (valueLowPass == null)
        {
            Init();
        }
        changeLowPass.Input(1);
        value = current/max;
    }

    public void AddValue(float Value)
    {
        if (valueLowPass == null)
        {
            Init();
        }
        changeLowPass.Input(1);
        value += Value;
    }
}
