
Shoot'em Up Music Set
===============

* Description
Shoot'em Up Music Set is a game music set package for Shoot'em Up Game. 
This package contains 9 high quality tracks for typical Shoot'em Up situations
such as opening, normal stage and boss stage.


* Format
44100Hz, 16bit, 2ch, wav


* Loop Point
All tracks have loop points for seamless connection arrangement and reverberation.

   filename                  title         loop start(sec)  loop end(sec)
  01_Shoot_A_Stage1.wav  Cosmic Cruise          8.509         59.709
  02_Shoot_A_Boss1.wav   Evil Dance            10.683         64.017
  03_Shoot_A_Stage2.wav  Highway Starship       6.512         76.913
  04_Shoot_A_Boss2.wav   Drum n "BASE"         13.426         45.425
  05_Shoot_B_Title.wav   Infinity Title        59.597        110.797
  06_Shoot_B_Stage1.wav  Infinity Stage1       67.193        132.875
  07_Shoot_B_Stage2.wav  Infinity Stage2       63.376        118.233
  08_Shoot_B_Stage3.wav  Infinity Stage3       86.818        129.484
  09_Shoot_B_Boss.wav    Infinity Boss         91.643        178.916

Unity default audio player does not support loop with intro section.
The following is an example code of loop audio player with loop point:

-- LoopPlayer.cs ------------------------------------------
using UnityEngine;

public class LoopPlayer : MonoBehaviour
{
    public AudioClip audioFile;
    public float volume;
    public float loopPointStart;
    public float loopPointEnd;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioFile;
        audioSource.volume = volume;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource.time > loopPointEnd)
        {
            audioSource.time = loopPointStart;
        }
    }
}
----------------------------------------------

* Credit
Copyright(C) 2019 SmileBoom Co.Ltd. All Rights Reserved.

