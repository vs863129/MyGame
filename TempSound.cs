using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSound : MonoBehaviour
{
    public void Play(AudioClip audioClip,float Volume,float Pitch)
    {
        AudioSource AudioSource=GetComponent<AudioSource>();
        AudioSource.volume = Volume;
        AudioSource.pitch = Pitch;
        AudioSource.PlayOneShot(audioClip);
        Destroy(gameObject, audioClip.length);
    }
}
