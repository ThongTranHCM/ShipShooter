using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePoolManager;
using ThongNguyen.PlayerController;
using WavePattern;

[System.Serializable]
public class WaveManager{
    [SerializeField]
    private PatternData patternData;
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private float healthMultiplier;
    [SerializeField]
    private float attackMultiplier;
    private int numMonsterRemaining;
    private int totalMonster;
    private bool isSpawned = false;
    private float startTime;
    private float endTime;

    public WaveManager(PatternData PatternData, EnemyData EnemyData, float HealthMultiplier, float AttackMultiplier){
        patternData = PatternData;
        enemyData = EnemyData;
        healthMultiplier = HealthMultiplier;
        attackMultiplier = AttackMultiplier;
        numMonsterRemaining = 0;
        startTime = Time.time;
        endTime = Time.time;
    }
    public void SpawnWave(float Z, float Padding){
        numMonsterRemaining = 0;
        totalMonster = 0;
        startTime = Time.time;
        bool mirror = patternData.mirror ? Random.value > 0.5f : false;
        foreach(SpawnInfo spawnInfo in patternData.enemyInstanceList){
            Vector3 start = GetSpawnPosition(spawnInfo.start.x, spawnInfo.start.y, Z + spawnInfo.timeDelay, Padding, mirror);
            Vector3 end = GetSpawnPosition(spawnInfo.end.x, spawnInfo.end.y, Z, Padding, mirror);
            GameObject enemyGameObject = SpawnMonster(enemyData, start).gameObject;
            enemyGameObject.transform.Rotate(mirror ? -patternData.rotation : patternData.rotation);
            LeanTween.cancel(enemyGameObject);
            LTDescr moveX_desc = LeanTween.moveX(enemyGameObject,end.x,patternData.duration).setDelay(spawnInfo.timeDelay);
            LTDescr moveY_desc = LeanTween.moveY(enemyGameObject,end.y,patternData.duration).setDelay(spawnInfo.timeDelay);
            switch (spawnInfo.pattern){
                case Pattern.CurveX:
                    moveX_desc.setEase(LeanTweenType.easeOutSine);
                    moveY_desc.setEase(LeanTweenType.easeInSine);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.CurveY:
                    moveX_desc.setEase(LeanTweenType.easeInSine);
                    moveY_desc.setEase(LeanTweenType.easeOutSine);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.Linear:
                    moveX_desc.setEase(LeanTweenType.linear);
                    moveY_desc.setEase(LeanTweenType.linear);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.ZigZag:
                    moveX_desc = LeanTween.moveX(enemyGameObject,end.x,patternData.duration/4).setEase(LeanTweenType.linear).setLoopPingPong(2).setDelay(spawnInfo.timeDelay);
                    moveY_desc.setEase(LeanTweenType.linear).setDelay(spawnInfo.timeDelay);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration/4));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.Dive:
                    moveX_desc.setEase(LeanTweenType.linear).setDelay(spawnInfo.timeDelay);
                    moveY_desc = LeanTween.moveY(enemyGameObject,end.y,patternData.duration/2).setEase(LeanTweenType.easeInSine).setLoopPingPong(1).setDelay(spawnInfo.timeDelay);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.RectX:
                    moveX_desc.setEase(LeanTweenType.easeOutCirc);
                    moveY_desc.setEase(LeanTweenType.easeInCirc);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.RectY:
                    moveX_desc.setEase(LeanTweenType.easeInCirc);
                    moveY_desc.setEase(LeanTweenType.easeOutCirc);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.BurstY:
                    moveX_desc.setEase(LeanTweenType.easeOutExpo);
                    moveY_desc.setEase(LeanTweenType.easeOutBack);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.BurstX:
                    moveX_desc.setEase(LeanTweenType.easeOutBack);
                    moveY_desc.setEase(LeanTweenType.easeOutExpo);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                case Pattern.BurstXY:
                    moveX_desc.setEase(LeanTweenType.easeOutBack);
                    moveY_desc.setEase(LeanTweenType.easeOutBack);
                    moveX_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveX_desc.id, patternData.duration));
                    moveY_desc.setOnUpdate((float x)=> UpdateSpeed(enemyGameObject, moveY_desc.id, patternData.duration));
                    break;
                default:
                    break;
            }
            List<int> patternTweensID = new List<int>();
            patternTweensID.Add(moveX_desc.id);
            patternTweensID.Add(moveY_desc.id);
            if(patternData.destroyOnComplete){
                moveX_desc.setOnComplete( () => RemoveEnemyOnFinishPattern(enemyGameObject, patternTweensID));
                moveY_desc.setOnComplete( () => RemoveEnemyOnFinishPattern(enemyGameObject, patternTweensID));
            }
            float tmp = startTime + patternData.duration + spawnInfo.timeDelay;
            endTime = endTime < tmp ? tmp : endTime;
        }
        isSpawned = true;
    }
    private Vector3 GetSpawnPosition(float X, float Y, float Z, float Padding, bool Mirror){
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0,0,Z));
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0,Camera.main.pixelHeight,Z));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth,0,Z));
        bottomLeft *= (1 + Padding);
        topLeft *= (1 + Padding);
        bottomRight *= (1 + Padding);
        float x = Mathf.Lerp(bottomLeft.x,bottomRight.x, Mirror? X : 1-X);
        float y = Mathf.Lerp(bottomLeft.y,topLeft.y,Y);
        return new Vector3(x,y,Z);
    }
    private Transform SpawnMonster(EnemyData Data, Vector3 StartPosition)
    {
        Transform monsterGo = null;
        GameObject lPrefab = Data.prefab;
        string poolName = Constants.poolMonster;

        if (string.IsNullOrEmpty(poolName))
        {
            monsterGo = Object.Instantiate(lPrefab).transform;
        }
        else if (poolName.Equals(Constants.poolMonster))
        {
            monsterGo = PoolManager.Pools[poolName].Spawn(lPrefab.transform);
            monsterGo.transform.position = StartPosition;
            //monsterGo.transform.rotation = transform.rotation;
        }
        else
        {
            monsterGo = PoolManager.Pools[poolName].Spawn(lPrefab.transform);
        }
        IEnemyController enemyController = monsterGo.GetComponent<IEnemyController>();
        enemyController.Install(healthMultiplier * Data.health, 0);
        if (Data.gunData != null)
        {
            enemyController.InstallGun(Data.gunObject, Data.gunData);
        }
        enemyController.SetWaveManager(this);
        numMonsterRemaining += 1;
        totalMonster += 1;
        return monsterGo;
    }
    public void OnMonsterDeath(IEnemyController enemy)
    {
        numMonsterRemaining -= 1;
    }
    public bool IsCleared(){
        return (numMonsterRemaining == 0) && IsSpawned();
    }
    public bool IsSpawned(){
        return isSpawned;
    }
    public float RemainPercentage(){
        return totalMonster != 0 ? (float)numMonsterRemaining / totalMonster : 0;
    }
    public float GetProgress(){
        return (Time.time - startTime) / (endTime - startTime);
    }
    public bool IsPatternFinished(List<int> TweensID){
        foreach(int id in TweensID){
            if(LeanTween.isTweening(id))
                return false;
        }
        return true;
    }
    private void RemoveEnemy(GameObject Enemy){
        Enemy.GetComponent<IEnemyController>().OnRemove();
    } 
    private void RemoveEnemyOnFinishPattern(GameObject Enemy, List<int> tweensID){
        if(IsPatternFinished(tweensID)){
            if(Enemy.activeSelf){
                RemoveEnemy(Enemy);
            }
        }
    }
    private void UpdateSpeed(GameObject Enemy, int TweenID, float Duration){
        float ratioPassed = LeanTween.descr(TweenID).ratioPassed;
        float speedPercentage = Enemy.GetComponent<IEnemyController>().GetSpeedPercentage();
        LeanTween.descr(TweenID).setTime(Duration / speedPercentage).setPassed(Duration * ratioPassed / speedPercentage);
    }
}