using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioUnit : NetworkBehaviour
{
    [SerializeField] private AudioClip schritte;
    [SerializeField] private AudioClip kampf1;
    [SerializeField] private AudioClip tod;


    [SerializeField] private AudioSource audioSource;

    [Command(requiresAuthority=false)]
    public void startAudio(int clip) {
        startRPCAudio(clip);
    }

    [ClientRpc]
    public void startRPCAudio(int clip) {
        audioSource.Stop();
        if(clip == 0 && schritte != null) { //Schritte Sound
            audioSource.clip = schritte;
            audioSource.Play();
        }else if(clip == 1 && kampf1 != null) {
            audioSource.clip = kampf1;
            audioSource.Play();
        }else if(clip == 2 && tod != null) {
            audioSource.clip = tod;
            audioSource.Play();
            StartCoroutine(WaitForSound());
        }
    }

    public IEnumerator WaitForSound() {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        if(GameObject.Find("GameManager").GetComponent<RoundManager>().id == GetComponent<UnitSprite>().id) GameObject.Find("PlayerManager").GetComponent<UnitManager>().syncStillExists(GetComponent<UnitSprite>().vec);
    }
}
