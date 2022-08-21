using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MathUtil;

public class TimeOutCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProCounter;
    private LowPassFilter _lowPassDif;
    private float _value = 9.9f;
    private float _roundValue;
    // Start is called before the first frame update
    void Start()
    {
        _roundValue = Mathf.Floor(_value);
        _lowPassDif = new LowPassFilter(0.1f,0);
    }

    void UpdateValue(){
        _value -= Time.unscaledDeltaTime;
        _value = Mathf.Max(_value,0);
        float dif = _roundValue - Mathf.Floor(_value);
        _roundValue -= dif;
        _textMeshProCounter.text = _roundValue.ToString();
        _lowPassDif.Input(dif);
        float lowScale = _lowPassDif.Output();
        this.transform.localScale = Vector3.one * (1 + 0.5f * lowScale / _lowPassDif.GetAlpha());
    }

    void EndCounter(){
        return;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateValue();
        if(Mathf.Floor(_value) == 0){
            EndCounter();
        }
    }
}
