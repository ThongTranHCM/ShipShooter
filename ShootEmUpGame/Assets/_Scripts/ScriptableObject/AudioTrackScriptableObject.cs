using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioTrackScriptableObject", order = 1)]
public class AudioTrackScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Track{
        public AudioClip audioClip;
        public string id;
        public bool loop = false;
        public float loopStart = 0;
        public float loopEnd = 0;
        public float volume = 1;
        public float interval = 0.1f;
        private float lastFire = 0.0f;
        public void ResetTimer(){
            lastFire = 0.0f;
        }
        public bool CanFire(){
            return Time.time - lastFire > interval;
        }
        public void Fire(){
            lastFire = Time.time;
        }
    }
    public List<Track> _TrackList;
    public GameObject _TrackSourcePrefab;

    public void CreateTrackSource(string ID){
        foreach (Track track in _TrackList){
            if (track.id == ID){
                if(track.CanFire()){
                    track.Fire();
                    GameObject trackSource = Object.Instantiate(_TrackSourcePrefab);
                    trackSource.GetComponent<TrackSource>().SetTrack(track);
                    return;
                }
            }
        }
        return;
    }

    public void ResetTimer(){
        foreach (Track track in _TrackList){
            track.ResetTimer();
        }
    }
}
