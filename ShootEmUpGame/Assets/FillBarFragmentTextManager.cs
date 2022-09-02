using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FillBarFragmentTextManager : FillBarManager.FillBarTextManager
{
    private int requirement = 0;
    public void SetValue(int Requirement){
        requirement = Requirement;
        UpdateText();
    }
    public override void UpdateText(){
        txtTMP.text = string.Format("{0}", Mathf.Floor(requirement * fillBarManager.GetAnimatedValue()));
    }
}