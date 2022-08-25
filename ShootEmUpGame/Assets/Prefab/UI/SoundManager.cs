using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    private AudioTrackScriptableObject _sfxTrackList;
    [SerializeField]
    private AudioTrackScriptableObject _bgmTrackList;
    public void PlaySFX(string ID){
        _sfxTrackList.CreateTrackSource(ID);
    }
    public void PlayBGM(string ID){
        _bgmTrackList.CreateTrackSource(ID);
    }
    public void ResetTimer(){
        _sfxTrackList.ResetTimer();
        _bgmTrackList.ResetTimer();
    }
    private void Awake(){
        _instance = this;
        
    }
}
