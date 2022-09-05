using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FillBarTimeTextManager : FillBarManager.FillBarTextManager
{
    private int requirement = 0;
    public void SetValue(int Requirement){
        requirement = Requirement;
        UpdateText();
    }
    public override void UpdateText(){
        int progress = Mathf.RoundToInt(requirement * (1 - fillBarManager.GetAnimatedValue()));
        TimeSpan span = TimeSpan.FromSeconds(progress);
        txtTMP.text = string.Format("{0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
    }
}
