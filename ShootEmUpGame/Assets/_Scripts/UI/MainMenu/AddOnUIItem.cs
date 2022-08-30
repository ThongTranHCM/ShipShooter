using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class AddOnUIItem : MonoBehaviour
{
    [SerializeField]
    private Image _imgAddOn;
    [SerializeField]
    private TextMeshProUGUI _txtAddOnLevel;
    [SerializeField]
    private FillBarManager _fillBarFragment;
    [SerializeField]
    private Button _btnClick;
    public System.Action onBtnClick;
    public void Install(string addOnText, Sprite sprAddOn, int level, int curFragment, int capFragment)
    {
        _txtAddOnLevel.text = "" + level;
        _fillBarFragment.SetRawValue(curFragment, capFragment);
        _imgAddOn.sprite = sprAddOn;
        _imgAddOn.color = (level == 0) ? Color.gray : Color.white;
    }
    public void OnItemClick()
    {
        onBtnClick?.Invoke();
    }
    public void UpdateFragment(int curFragment, int capFragment, float duration = 0.5f, float delay = 0.0f){
        _fillBarFragment.UpdateRawValue(curFragment, capFragment, duration, delay);
    }
}
