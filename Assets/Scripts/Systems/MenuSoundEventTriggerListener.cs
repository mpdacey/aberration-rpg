using UnityEngine;
using Cryptemental.Audio;

public class MenuSoundEventTriggerListener : MonoBehaviour
{
    public AudioClip menuMoveClip;
    public AudioClip menuConfirmClip;
    public AudioClip menuCancelClip;

    public void OnButtonMove()
    {
        AudioManager.PlayAudioClip(menuMoveClip);
    }

    public void OnButtonSubmit()
    {
        AudioManager.PlayAudioClip(menuConfirmClip);
    }

    public void OnButtonCancel()
    {
        AudioManager.PlayAudioClip(menuCancelClip);
    }
}
