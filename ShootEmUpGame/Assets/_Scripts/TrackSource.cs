using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSource : MonoBehaviour
{
    public AudioTrackScriptableObject.Track _track;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_track.loop == true && audioSource.time > _track.loopEnd)
        {
            if (_track.loopEnd > 0){
                audioSource.time = _track.loopStart;
            }
        }

        if (!audioSource.isPlaying){
            Destroy(gameObject);
        }
    }

    public void SetTrack(AudioTrackScriptableObject.Track Track){
        _track = Track;
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = Track.audioClip;
        audioSource.volume = Track.volume;
        audioSource.Play();
        if (_track.loop == true && _track.loopEnd == 0){
            audioSource.loop = true;
        }
    }
}
