using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioTrackScriptableObject _sfxTrackList;
    public AudioTrackScriptableObject _bgmTrackList;
    public void PlaySFX(string ID){
        _sfxTrackList.CreateTrackSource(ID);
    }
    public void PlayBGM(string ID){
        _bgmTrackList.CreateTrackSource(ID);
    }
}
