using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThongNguyen.PlayerController;

[CreateAssetMenu(fileName = "Data", menuName = "Data/RandomLevelDesignData", order = 1)]
public class RandomLevelDesignData : LevelDesignData
{
    [System.Serializable]
    public class AddOnDropPool{
        [SerializeField]
        private List<AddOnEquipData.AddOnType> addOnDropList;
        public AddOnDropPool(List<AddOnEquipData.AddOnType> AddOnDropList){
            addOnDropList = AddOnDropList;
        }
        public AddOnEquipData.AddOnType GetRandomAddOn(){
            return addOnDropList[Random.Range(0, addOnDropList.Count)];
        }
    }
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
    private List<AddOnDropPool> addOnDropPoolList;
    [SerializeField]
    private float dropRateMultiplier = 1;
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
                                List<AddOnDropPool> AddOnDropPoolList,
                                float DropRate){
        enemyDataPool = EnemyDataPool;
        numWaveList = NumWave;
        baseHealth = BaseHP;
        startHealthMultiplier = StartHP;
        endHealthMultiplier = EndHP;
        startDensity = StartDense;
        endDensity = EndDense;
        addOnDropPoolList = AddOnDropPoolList;
        dropRateMultiplier = DropRate;
        generatedWaveList = new List<WaveManager>();
        generatedDensityList = new List<float>();
        remainIndexList = new List<int>();
        generatedAddOnDropList = new List<AddOnEquipData.AddOnType>();
        curWave = 0;
        curDrop = 0;
    }
#if UNITY_EDITOR
    public static RandomLevelDesignData CreateInstance(List<EnemyData> EnemyDataPool,
                                int NumWave, 
                                float BaseHP, 
                                float StartHP, 
                                float EndHP, 
                                float StartDense, 
                                float EndDense, 
                                List<AddOnDropPool> AddOnDropPoolList,
                                float DropRate){
        RandomLevelDesignData data = ScriptableObject.CreateInstance<RandomLevelDesignData>();
        data.Init(EnemyDataPool, NumWave, BaseHP, StartHP, EndHP, StartDense, EndDense, AddOnDropPoolList, DropRate);
        return data;
    }
#endif
    public override IEnumerator InstallWaves()
    {
        GenerateWaves();
        GeneratedAddOnDrops();
        float denseSum = 0;
        while(curWave < generatedWaveList.Count){
            denseSum = 0;
            remainIndexList.RemoveAll(x => generatedWaveList[x].IsCleared());
            foreach(int remainIndex in remainIndexList){
                denseSum += Mathf.Exp(generatedDensityList[remainIndex]) * generatedWaveList[remainIndex].RemainPercentage();
            }
            if(denseSum + Mathf.Exp(generatedDensityList[curWave]) <= 1){
                generatedWaveList[curWave].SpawnWave(ZOffset, padding);
                remainIndexList.Add(curWave);
                denseSum += generatedDensityList[curWave];
                curWave += 1;
            }
            yield return Yielder.Get(Time.fixedDeltaTime);
        }
        while(!IsCompleted()){
            yield return Yielder.Get(Time.fixedDeltaTime);
        }
    }

    public bool IsCompleted(){
        foreach(WaveManager wave in generatedWaveList){
            if(!wave.IsCleared()){
                return false;
            }
        }
        return true;
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

    private void GeneratedAddOnDrops(){
        curDrop = 0;
        generatedAddOnDropList = new List<AddOnEquipData.AddOnType>();
        foreach(AddOnDropPool pool in addOnDropPoolList){
            generatedAddOnDropList.Add(pool.GetRandomAddOn());
        }
        Debug.Log(generatedAddOnDropList.Count);
    }

    public override GameObject DropOnKill(ThongNguyen.PlayerController.IEnemyController enemy){
        int index = Mathf.FloorToInt((generatedAddOnDropList.Count + 2) * GetProgress() * dropRateMultiplier);
        if (index >= 1 && index <= generatedAddOnDropList.Count && curDrop < index) {
            ItemController item = GamePlayManager.Instance.ItemSpawner.CreateAddOn(generatedAddOnDropList[index - 1], enemy.transform.position);
            item.onOutCamera += () =>
            {
                curDrop -= 1;
            };
            curDrop += 1;
        }
        return null;
    }

    public override List<EnemyData> GetEnemyDataList(){
        return enemyDataPool;
    }
}