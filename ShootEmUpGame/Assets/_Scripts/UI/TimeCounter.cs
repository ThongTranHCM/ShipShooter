using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    [SerializeField]
    private float _counter;
    private TimeSpan _timeSpan;
    [SerializeField]
    private TextMeshProUGUI _textMeshGUITime;
    void Start()
    {
        InitCounter();
    }

    private void InitCounter(){
        _counter = 20;
        _timeSpan = TimeSpan.FromSeconds(_counter);
    }

    private void UpdateCounter(){
        _counter -= Time.deltaTime;
        _counter = Mathf.Max(_counter,0);
        _timeSpan = TimeSpan.FromSeconds(_counter);
        _textMeshGUITime.text = string.Format("{0:D2}:{1:D2}", _timeSpan.Minutes, _timeSpan.Seconds);
    }

    private void EndCounter(){
        GamePlayManager.Instance.UIManager.ShowTimeOutScreen();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCounter();
        if (_counter == 0){
            EndCounter();
        }
    }
}
