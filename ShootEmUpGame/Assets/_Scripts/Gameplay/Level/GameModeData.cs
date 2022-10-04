using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameModeData : ScriptableObject
{
    public abstract LevelDesignData GetLevelData();


    public abstract IEnumerator StartGame();

    public abstract IEnumerator OnWinGame();

    public abstract IEnumerator OnLoseGame();
}
