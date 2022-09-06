using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillBarBaseTextManager : FillBarManager.FillBarTextManager
{
    private int requirement = 0;
    public void SetValue(int Requirement){
        requirement = Requirement;
        UpdateText();
    }
    public override void UpdateText(){
        txtTMP.text = string.Format("{0}/{1}", Mathf.Floor(requirement * fillBarManager.GetAnimatedValue()), requirement);
    }
}
