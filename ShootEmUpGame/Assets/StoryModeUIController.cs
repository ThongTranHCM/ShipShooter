using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryModeUIController : GameModeUIManager
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void UpdateLevelDesignData(){
        return;
    }
    public override void ResetUI(){
        DisplayEnemies();
        return;
    }
}
