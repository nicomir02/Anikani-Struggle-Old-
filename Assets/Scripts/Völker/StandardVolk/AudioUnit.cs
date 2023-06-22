using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUnit : MonoBehaviour
{
    [SerializeField] private AudioClip schritte;
    [SerializeField] private AudioClip kampf1;


    [SerializeField] private AudioSource audioSource;

    public void startAudio(int clip) {
        audioSource.Stop();
        if(clip == 0 && schritte != null) { //Schritte Sound
            audioSource.clip = schritte;
            audioSource.Play();
        }else if(clip == 1 && kampf1 != null) {
            audioSource.clip = kampf1;
            audioSource.Play();
        }
    }
}
