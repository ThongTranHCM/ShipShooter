using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/MissionListData", order = 1)]
public class MissionListData : ScriptableObject
{
    [System.Serializable]
    public class Mission{
        [SerializeField]
        private string key;
        [SerializeField]
        private string description;
        [SerializeField]
        private int requirement;
        [SerializeField]
        private int cooldown;
        [SerializeField]
        private int reward;

        private int start;
        private int timeComplete;

        private void Reset(){
            int value = 0;
            start = value;
        }

        public float GetProgress(){
            int value = 0;
            return (value - start) / requirement;
        }

        public void Goto(){
            return;
        }

        public void Complete(){
            Reset();
            timeComplete =  System.DateTime.Now.Second;
        }

        public int GetReward(){
            return reward;
        }

        public bool IsOnCoolDown(){
            return System.DateTime.Now.Second - timeComplete < cooldown;
        }
    }
    [SerializeField]
    private List<Mission> missionList;
}
