using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WavePattern;
using GamePoolManager;
using ThongNguyen.PlayerController;

public class LevelDesignData : ScriptableObject
{
    public float ZOffset = 0;
    public float padding = 0.2f;

    public virtual IEnumerator InstallWaves()
    {
        yield return null;
    }

    public virtual IEnumerator StartGame(){
        yield return null;
    }

    public virtual void LoseGame(){
        GamePlayManager.Instance.QuitGame();
    }

    public virtual float GetProgress(){
        return 0;
    }

    public virtual GameObject DropOnKill(ThongNguyen.PlayerController.IEnemyController enemy){
        return null;
    }

    public virtual List<EnemyData> GetEnemyDataList(){
        return null;
    }
}
