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
    public System.Action onBtnClick;

    [SerializeField]
    private Sprite _sprSelected;
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
        _imgMain.sprite = _sprSelected;
        _txtLevel.text = _intLevel.ToString();
        _txtLabel.text = "Selected";
    }

    public void ShowAsShip()
    {
        _imgMain.sprite = _sprShip;
        _txtLevel.text = _intLevel.ToString();
        _txtLabel.text = _strName;
    }
    public void OnUIClick()
    {
        onBtnClick?.Invoke();
    }
}
