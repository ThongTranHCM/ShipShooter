using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MathUtil;

public class TimeOutCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshProCounter;
    private float _value = 6.9f;
    private int _roundValue;
    private bool _end = false;
    // Start is called before the first frame update
    void Start()
    {
        _roundValue = Mathf.FloorToInt(_value);
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown(){
        while(_roundValue != 0){
            _value -= Time.unscaledDeltaTime;
            _value = Mathf.Max(_value,0);
            int dif = _roundValue - Mathf.FloorToInt(_value);
            if( dif > 0){
                LeanTween.scale(_textMeshProCounter.gameObject, Vector3.one, 0.0f);
                LeanTween.scale(_textMeshProCounter.gameObject, Vector3.one * 1.5f, 0.25f).setEase(LeanTweenType.punch);
            }
            _roundValue -= dif;
            _textMeshProCounter.text = Mathf.Max(_roundValue - 1, 0).ToString();
            yield return Yielder.Get(Time.unscaledDeltaTime);
        }
        GamePlayManager.Instance.LoseGame();
        yield return null;
    }
}
