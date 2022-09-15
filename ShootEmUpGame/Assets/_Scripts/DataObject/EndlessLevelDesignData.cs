using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThongNguyen.PlayerController;

[CreateAssetMenu(fileName = "Data", menuName = "Data/EndlessLevelDesignData", order = 1)]
public class EndlessLevelDesignData : LevelDesignData
{
    [SerializeField]
    private List<EnemyData> enemyDataPool;
    [SerializeField]
    private int numWaveList;
    [SerializeField]
    private float baseHealth = 1;
    [SerializeField]
    private float startHealthMultiplier = 1;
    [SerializeField]
    private float endHealthMultiplier = 1;
    [SerializeField]
    private float startDensity = 1;
    [SerializeField]
    private float endDensity = 1;
    [SerializeField]
    private float bonusProb = 1;
    [SerializeField]
    private float bonusGold = 1;

    private List<AddOnEquipData.AddOnType> generatedAddOnDropList;
    private List<WaveManager> generatedWaveList;
    private List<float> generatedDensityList;
    private List<int> remainIndexList;
    private int curWave = 0;
    private int curDrop = 0;

    private void Init(List<EnemyData> EnemyDataPool,
                                int NumWave, 
                                float BaseHP, 
                                float StartHP, 
                                float EndHP, 
                                float StartDense, 
                                float EndDense,
                                float DropRate){
        enemyDataPool = EnemyDataPool;
        numWaveList = NumWave;
        baseHealth = BaseHP;
        startHealthMultiplier = StartHP;
        endHealthMultiplier = EndHP;
        startDensity = StartDense;
        endDensity = EndDense;
        generatedWaveList = new List<WaveManager>();
        generatedDensityList = new List<float>();
        remainIndexList = new List<int>();
        curWave = 0;
        curDrop = 0;
    }
#if UNITY_EDITOR
    public static EndlessLevelDesignData CreateInstance(List<EnemyData> EnemyDataPool,
                                int NumWave, 
                                float BaseHP, 
                                float StartHP, 
                                float EndHP, 
                                float StartDense, 
                                float EndDense, 
                                float DropRate){
        EndlessLevelDesignData data = ScriptableObject.CreateInstance<EndlessLevelDesignData>();
        data.Init(EnemyDataPool, NumWave, BaseHP, StartHP, EndHP, StartDense, EndDense, DropRate);
        return data;
    }
#endif
    public override IEnumerator InstallWaves()
    {
        GenerateWaves();
        Debug.LogError("Install");
        float denseSum = 0;
        while(!IsCompleted()){
            denseSum = 0;
            remainIndexList.RemoveAll(x => generatedWaveList[x].IsCleared());
            foreach(int remainIndex in remainIndexList){
                denseSum += Mathf.Exp(generatedDensityList[remainIndex]) * generatedWaveList[remainIndex].RemainPercentage();
            }
            if(denseSum + Mathf.Exp(generatedDensityList[curWave]) <= 1){
                if( curWave >= generatedWaveList.Count){
                    AddWave(curWave);
                }
                generatedWaveList[curWave].SpawnWave(ZOffset, padding);
                remainIndexList.Add(curWave);
                denseSum += generatedDensityList[curWave];
                curWave += 1;
            }
            yield return Yielder.Get(Time.fixedDeltaTime);
        }
    }

    public bool IsCompleted(){
        return false;
    }

    public override float GetProgress(){
        float count = 0;
        foreach(WaveManager wave in generatedWaveList){
            count += wave.IsSpawned() ? 1 - wave.RemainPercentage() : 0; 
        }
        return count / generatedWaveList.Count;
    }

    private void GenerateWaves(){
        curWave = 0;
        generatedWaveList = new List<WaveManager>();
        generatedDensityList = new List<float>();
        remainIndexList = new List<int>();
        for(int i = 0; i < numWaveList; i++){
            float density = Mathf.Lerp(Mathf.Log(1 / startDensity), Mathf.Log(1 / endDensity), (float) i /  numWaveList);
            EnemyData randomEnemyData = enemyDataPool[Random.Range(0, enemyDataPool.Count)];
            PatternData randomPatternData = randomEnemyData.PatternDataList[Random.Range(0, randomEnemyData.PatternDataList.Count)];
            WaveManager randomWave = new WaveManager(randomPatternData, randomEnemyData, Mathf.Lerp(startHealthMultiplier * baseHealth, endHealthMultiplier * baseHealth, (float)i / numWaveList), 0);
            generatedDensityList.Add(density);
            generatedWaveList.Add(randomWave);
        }
    }

    private void AddWave(int i){
        float density = Mathf.Log(1 / endDensity);
        EnemyData randomEnemyData = enemyDataPool[Random.Range(0, enemyDataPool.Count)];
        PatternData randomPatternData = randomEnemyData.PatternDataList[Random.Range(0, randomEnemyData.PatternDataList.Count)];
        WaveManager randomWave = new WaveManager(randomPatternData, randomEnemyData, Mathf.Lerp(startHealthMultiplier, endHealthMultiplier, (float)i / numWaveList), 0);
        generatedDensityList.Add(density);
        generatedWaveList.Add(randomWave);
    }

    public override List<EnemyData> GetEnemyDataList(){
        return enemyDataPool;
    }

    public override IEnumerator StartGame(){
        if(TimeChestManager.Instance != null){
            TimeChestManager.Instance.ProgressMission("play_game",1);
        }
        return GamePlayManager.Instance.StartGame();
    }

    public override IEnumerator EndGame(){
        if(TimeChestManager.Instance != null){
            TimeChestManager.Instance.ProgressMission("clear_stage",1);
        }
        DataManager.Instance.LastLevelWin++;
        GamePlayManager.Instance.RewardCollect();
        return GamePlayManager.Instance.EndGame();
    }

    public override void LoseGame(){
        GamePlayManager.Instance.RewardCollect();
        GamePlayManager.Instance.QuitGame();
    }
}