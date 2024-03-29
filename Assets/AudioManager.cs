using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioSource audioSource;
    private static AudioMixerManager audioMixer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioMixer = GetComponent<AudioMixerManager>();
    }

    public static void PlayAudioClip(AudioClip audioClip, bool randomPitch = false)
    {
        audioMixer.SetSFXPitch(randomPitch ? Random.Range(0.95f, 1.05f) : 1);
        audioSource.PlayOneShot(audioClip);
    }
}
