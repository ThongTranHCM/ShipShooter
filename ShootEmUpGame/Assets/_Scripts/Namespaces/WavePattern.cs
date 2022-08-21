using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WavePattern{
    public enum Pattern {CurveX, CurveY, Linear, ZigZag, Dive, RectX, RectY, BurstX, BurstY, BurstXY};
    public enum Difficulty {Easy,Normal,Hard,Extreme};
    [System.Serializable]
    public class SpawnInfo{
        public Vector2 start;
        public Vector2 end;
        public float timeDelay;
        public Pattern pattern;
        public SpawnInfo(GameObject Enemy, Vector2 Start, Vector2 End, float TimeDelay, Pattern Pattern){
            start = Start;
            end = End;
            timeDelay = TimeDelay;
            pattern = Pattern;
        }
    }
}