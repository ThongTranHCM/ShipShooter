using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddOnInfoCanvasManager : MonoBehaviour
{
    private static AddOnInfoCanvasManager instance;
    public static AddOnInfoCanvasManager Instance
    {
        get { return instance; }
    }
    [SerializeField]
    private GameObject content;
    private IAddOnData _addOnData;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI _txtAddOnName;
    [SerializeField]
    private TextMeshProUGUI _txtAddOnLevel;
    [SerializeField]
    private TextMeshProUGUI _txtAddOnDesc;
    [SerializeField]
    private TextMeshProUGUI _txtAddOnBonus;
    [SerializeField]
    private FillBarManager _fillBarFragment;
    [SerializeField]
    private Image _imgBtnPositive;
    [SerializeField]
    private TextMeshProUGUI _txtBtnPositive;

    // Start is called before the first frame update
    public AddOnInfoCanvasManager()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetContentShow(Sprite spr, string addOnName, int level, string desc, string levelBonus)
    {
        icon.sprite = spr;
        _txtAddOnName.text = addOnName;
        _txtAddOnLevel.text = "Level " + level;
        _txtAddOnDesc.text = desc;
        _txtAddOnBonus.text = levelBonus;
    }

    public void SetFillbar(int currentFragment, int maxValue)
    {
        _fillBarFragment.Init();
        ((FillBarFragmentTextManager)_fillBarFragment.GetFillBarTextManager()).SetValue(maxValue);
        _fillBarFragment.SetRawValue(12, 18);
    }
    public void SetContentShow(IAddOnData addOnData)
    {
        _addOnData = addOnData;
        int level = addOnData.GetLevel;
        SetContentShow(addOnData.GetSprite, addOnData.GetAddOnType.ToString(), level, "This is Desc", "This is Bonus");
        int cost = 0;
        if (level < 1)
        {
            cost = GameInformation.Instance.addOnEquipData.GetUnlockCost();
            _txtBtnPositive.text = "Unlock";
        }
        else
        {
            cost = GameInformation.Instance.addOnEquipData.GetUpgradeCost(level);
            _txtBtnPositive.text = "Upgrade";
        }
        SetFillbar(addOnData.GetFragment, cost);
        if (addOnData.GetFragment < cost)
        {
            _imgBtnPositive.color = GameInformation.Instance.materialData.colInvalidBtn;
        }
        else
        {
            _imgBtnPositive.color = GameInformation.Instance.materialData.colPositiveBtn;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnClickBackground()
    {
        AddOnInfoCanvasManager.Instance.Close();
    }
    public void OnClickPositiveBtn()
    {
        AddOnInfoCanvasManager.Instance.Close();
        if (_addOnData.GetLevel < 1)
        {
            DataManager.Instance.addOnUserData.Upgrade(_addOnData.GetAddOnType);
        }
        else
        {
            DataManager.Instance.addOnUserData.Unlock(_addOnData.GetAddOnType);
        }
        FragmentRewardCanvasManager.Instance.Show(_addOnData.GetAddOnType.ToString(), 0);
    }
    public void OnClickCloseBtn()
    {
        AddOnInfoCanvasManager.Instance.Close();
    }
}