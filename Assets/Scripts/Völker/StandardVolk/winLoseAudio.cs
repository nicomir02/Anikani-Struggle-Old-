using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class winLoseAudio : NetworkBehaviour
{
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;

    [SerializeField] private AudioSource audioSource;

    public void startAudio(int clip) {
        startRPCAudio(clip);
    }

    public void startRPCAudio(int clip) {
        audioSource.Stop();
        if(clip == 0 && win != null) { //Schritte Sound
            audioSource.clip = win;
            audioSource.Play();
        }else if(clip == 1 && lose != null) {
            audioSource.clip = lose;
            audioSource.Play();
        }
    }
}