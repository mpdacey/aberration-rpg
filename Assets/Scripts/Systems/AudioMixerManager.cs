using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    const float FULL_DECIBEL = 80;
    const string MUSIC_VOLUME_KEY = "_MusicVolume";
    const string SOUND_VOLUME_KEY = "_SFXVolume";
    const string SOUND_PITCH_KEY = "_SFXPitch";

    // Volume Settings
    public void SetMusicVolume(float level) {
        mixer.SetFloat(MUSIC_VOLUME_KEY, Mathf.Sqrt(level) * FULL_DECIBEL - FULL_DECIBEL);
    }

    public void SetSFXVolume(float level) =>
        mixer.SetFloat(SOUND_VOLUME_KEY, Mathf.Sqrt(level) * FULL_DECIBEL - FULL_DECIBEL);

    // Pitch Settings
    public void SetSFXPitch(float level) =>
        mixer.SetFloat(SOUND_PITCH_KEY, level);

}
