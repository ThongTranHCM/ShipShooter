using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelBarController : MonoBehaviour
{
    public System.Action<int> onLevelUp;

    public Animator anim;
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtLevel;
    public Slider sliderExp;

    [SerializeField]
    private int _exp;
    [SerializeField]
    private int _level = 1;
    public void SetLevel(int level, int exp, bool animated = false)
    {
        if (animated)
        {
            float current = this._level + (float)this._exp / GameInformation.Instance.GetExpForLevel(this._level);
            float next = level + exp / (float)GameInformation.Instance.GetExpForLevel(level);
            if (next > GameInformation.Instance.maxLevel())
                next = GameInformation.Instance.maxLevel();
            this._exp = exp;
            this._level = level;
            anim.SetBool("GainExp", true);
        }
        else
        {
            UpdateUI(level, exp);
            this._exp = exp;
            this._level = level;
        }
    }
    private void UpdateUI(int level, int exp)
    {
        _txtLevel.text = level.ToString();
        if (GameInformation.Instance.isLevelMaxed(level))
        {
            int totalExp = GameInformation.Instance.GetExpForLevel(level - 1);
            //_txtExp.text = string.Format("{0}/{1}", totalExp, totalExp);
            sliderExp.normalizedValue = 1;
        }
        else
        {
            int totalExp = GameInformation.Instance.GetExpForLevel(level);
            int earnedExp = exp;
            //_txtExp.text = string.Format("{0}/{1}", earnedExp, totalExp);
            float percent = (float)exp / GameInformation.Instance.GetExpForLevel(level);
            sliderExp.normalizedValue = percent;
        }
    }
}
