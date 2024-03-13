using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    // Volume Settings
    public void SetMasterVolume(float level) =>
        mixer.SetFloat("_MasterVolume", Mathf.Log10(level) * 20);

    // Pitch Settings
    public void SetSFXPitch(float level) =>
        mixer.SetFloat("_PlayerSFXPitch", level);
}
