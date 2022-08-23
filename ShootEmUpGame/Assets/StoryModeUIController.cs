using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryModeUIController : GameModeUIManager
{
    [SerializeField]
    private TextMeshProUGUI _txt;
    protected override void UpdateLevelDesignData(){
        return;
    }
    public override void ResetUI(){
        DisplayEnemies();
        return;
    }

    public void Start()
    {
        _txt.text = "Level " + (DataManager.Instance.LastLevelWin + 1);
    }
}
