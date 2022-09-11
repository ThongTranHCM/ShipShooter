using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipSelectUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtLevel;
    [SerializeField]
    private TextMeshProUGUI _txtLabel;
    [SerializeField]
    private Image _imgMain;
    [SerializeField]
    private Image _imgCheck;
    [SerializeField]
    private Image _imgOverlay;
    [SerializeField]
    private Image _imgLock;
    public System.Action onBtnClick;
    private Sprite _sprShip;

    private string _strName;

    private int _intLevel;

    public void Install(Sprite sprShip, int iLevel, string strName)
    {
        _sprShip = sprShip;
        _strName = strName;
        _intLevel = iLevel;
    }

    public void ShowAsSelected()
    {
        _imgCheck.gameObject.SetActive(true);
        _imgMain.gameObject.SetActive(false);
        if (_intLevel > 0)
        {
            _imgLock.gameObject.SetActive(false);
            _txtLevel.gameObject.SetActive(true);
            _imgOverlay.gameObject.SetActive(false);
            _txtLevel.text = _intLevel.ToString();
        }
        else
        {
            _imgLock.gameObject.SetActive(true);
            _txtLevel.gameObject.SetActive(false);
            _imgOverlay.gameObject.SetActive(true);
        }
        _txtLabel.text = "Selected";
    }

    public void ShowAsShip()
    {
        _imgCheck.gameObject.SetActive(false);
        _imgMain.gameObject.SetActive(true);
        _imgMain.sprite = _sprShip;
        if (_intLevel > 0)
        {
            _imgLock.gameObject.SetActive(false);
            _txtLevel.gameObject.SetActive(true);
            _imgOverlay.gameObject.SetActive(false);
            _txtLevel.text = _intLevel.ToString();
        }
        else
        {
            _imgLock.gameObject.SetActive(true);
            _txtLevel.gameObject.SetActive(false);
            _imgOverlay.gameObject.SetActive(true);
        }
        _txtLabel.text = _strName;
    }
    public void OnUIClick()
    {
        onBtnClick?.Invoke();
    }
}
