using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ShipHealthBar _shipHealthBar;

    [SerializeField]
    private ScoreValueUpdate _scoreValue;
    [SerializeField]
    private ScoreValueUpdate _goldValue;

    //Editor Only
    [SerializeField]
    private Text[] _txtFps;
    [SerializeField]
    [Range(1,30)]
    private int[] _numFrame;
    private int currentIndex;

    private float[] _arrayTime;
    private float[] _flSumTime;

    [Header("Screens")]
    [SerializeField]
    private GameObject _gameLoseScreen;
    [SerializeField]
    private GameObject _timeOutScreen;
    [SerializeField]
    private GameObject _pauseScreen;
    [SerializeField]
    private GameObject _gameWinScreen;

    [Header("Canvas")]
    [SerializeField]
    private GameObject _victoryCanvas;
    [SerializeField]
    private GameObject _tintCanvas;

    [Header("Tint Color")]
    [SerializeField]
    private Color _hitTint;

    public void InstallLives(int iLifeCount)
    {
        _shipHealthBar.Install(iLifeCount);
    }
    public void UpdateLives(int iLifeCount)
    {
        _shipHealthBar.UpdateLives(iLifeCount);
    }

    public void AddScore(int baseValue, int destValue)
    {
        _scoreValue.UpdateProgress(baseValue, destValue);
    }

    public void AddGold(int baseValue, int destValue)
    {
        _goldValue.UpdateProgress(baseValue, destValue);
    }

    public void Awake()
    {
        _arrayTime = new float[30];
        _flSumTime = new float[_txtFps.Length];
        for (int i = 0; i < 30; i++)
        {
            _arrayTime[i] = Time.deltaTime;
        }
        for (int i = 0; i < _txtFps.Length; i++)
        {
            _flSumTime[i] = _numFrame[i] * Time.deltaTime;
        }
    }
    public void Update()
    {
        float _aveFps = 0;
        float _curTime = Time.deltaTime;
        int _oldFrameIndex = 0;
        for (int i = 0; i < _txtFps.Length; i++)
        {
            _aveFps = (_numFrame[i] / _flSumTime[i]);
            _txtFps[i].text = "FPS " + _numFrame[i] + " : " + (int)(_aveFps);
            _txtFps[i].color = new Color(Mathf.Clamp((_aveFps - 150) / 300, 0, 1), 1, Mathf.Clamp((450 - _aveFps) / 300, 0, 1));
        }
        for (int i = 0; i < _txtFps.Length; i++)
        {
            _oldFrameIndex = (30 + currentIndex - _numFrame[i]) % 30;
            _flSumTime[i] += _curTime - _arrayTime[_oldFrameIndex];
        }
        _arrayTime[currentIndex] = _curTime;
        currentIndex = (currentIndex + 1) % 30;
    }
    #region Show/Hide Screens
    public void ShowWinScreen(bool isHide = false)
    {
        _gameWinScreen.SetActive(!isHide);
    }

    public void ShowLoseScreen(bool isHide = false)
    {
        _gameLoseScreen.SetActive(!isHide);
    }

    public void ShowTimeOutScreen(bool isHide = false)
    {
        _timeOutScreen.SetActive(!isHide);
    }

    public void ShowPauseScreen(bool isHide = false)
    {
        _pauseScreen.SetActive(!isHide);
    }

    public void PlayVictory()
    {
        LTSeq seq = LeanTween.sequence();
        seq.append(() => {
            LeanTween.scaleY(_victoryCanvas, 1, 1.0f).setEase(LeanTweenType.easeOutBack);
            LeanTween.scaleX(_victoryCanvas, 1, 1.0f).setEase(LeanTweenType.easeOutBack);
            seq.append(0.75f);
        });
        seq.append(3f);
        seq.append(() => {
            LeanTween.scaleY(_victoryCanvas, 0, 0.5f).setEase(LeanTweenType.easeInQuint);
            LeanTween.scaleX(_victoryCanvas, 0, 0.5f).setEase(LeanTweenType.easeInQuint);
            seq.append(0.5f);
        });
    }

    public void PlayGotHit(){
        Debug.Log("Start PlayGotHit");
        Color clearTint = new Color(_hitTint.r,_hitTint.g,_hitTint.b,0);
        LTSeq seq = LeanTween.sequence();
        seq.append(() => {_tintCanvas.SetActive(true);});
        seq.append(LeanTween.value(_tintCanvas, 
                                    (Color c) => {_tintCanvas.GetComponent<Image>().color = c;},
                                    clearTint, _hitTint, 0f));
        seq.append(LeanTween.value(_tintCanvas, 
                                    (Color c) => {_tintCanvas.GetComponent<Image>().color = c;},
                                    _hitTint , clearTint, 0.75f).setEase(LeanTweenType.easeOutBack));
        seq.append(() => {_tintCanvas.SetActive(false);});
        Debug.Log("Finish PlayGotHit");
    }
    #endregion
}
