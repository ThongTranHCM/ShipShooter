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
    public void SetContentShow(IAddOnData addOnData)
    {
        _addOnData = addOnData;
        SetContentShow(addOnData.GetSprite, addOnData.GetAddOnType.ToString(), addOnData.GetLevel, "This is Desc", "This is Bonus");
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
        DataManager.Instance.addOnUserData.Upgrade(_addOnData.GetAddOnType);
        Debug.LogError("Allow Upgrade");
        FragmentRewardCanvasManager.Instance.Show(_addOnData.GetAddOnType.ToString(), 0);
    }
    public void OnClickNegativeBtn()
    {
        AddOnInfoCanvasManager.Instance.Close();
        DataManager.Instance.addOnUserData.Upgrade(_addOnData.GetAddOnType);
        Debug.LogError("Allow Upgrade");
        FragmentRewardCanvasManager.Instance.Show(_addOnData.GetAddOnType.ToString(), 0);
    }
}