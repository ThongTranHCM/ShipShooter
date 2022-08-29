using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WavePattern;
using GamePoolManager;
using ThongNguyen.PlayerController;

[CreateAssetMenu(fileName = "Data", menuName = "Data/ScriptedLevelDesignData", order = 1)]
public class ScriptedLevelDesignData : LevelDesignData
{
    [System.Serializable]
    public class TimeStamp{
        [SerializeField]
        private WaveManager wave;
        [SerializeField]
        private float delay;
        
        public float GetDelay(){
            return delay;
        }

        public WaveManager GetWave(){
            return wave;
        }

        public TimeStamp(float Delay, WaveManager Wave){
            delay = Delay;
            wave = Wave;
        }
    }
    [SerializeField]
    private List<TimeStamp> stampList;
    public override IEnumerator InstallWaves()
    {
        int index = 0;
        float sinceStartTime = 0;
        float sinceLastWave = 0;
        Debug.LogError("Install");
        while( index < stampList.Count){
            if (sinceStartTime > stampList[index].GetDelay() && stampList[index].GetDelay() >= 0) {
                stampList[index].GetWave().SpawnWave(ZOffset, padding);
                index++;
            } else if (index >= 1) {
                if (stampList[index - 1].GetWave().IsCleared() && -sinceLastWave < stampList[index].GetDelay()){
                    stampList[index].GetWave().SpawnWave(ZOffset, padding);
                    index++;
                    sinceLastWave = 0;
                }
            }
            sinceStartTime += Time.fixedDeltaTime;
            sinceLastWave += Time.fixedDeltaTime;
            yield return Yielder.Get(Time.fixedDeltaTime);
        }
    }
}
