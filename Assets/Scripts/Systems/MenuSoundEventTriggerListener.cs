using UnityEngine;
using UnityEngine.EventSystems;
using Cryptemental.Audio;

public class MenuSoundEventTriggerListener : MonoBehaviour
{
    public AudioClip menuMoveClip;
    public AudioClip menuConfirmClip;
    public AudioClip menuCancelClip;

    private GameObject lastSelected = null;

    public void OnButtonMove()
    {
        EventSystem currentSystem = EventSystem.current;
        if (currentSystem.currentSelectedGameObject == lastSelected) return;

        AudioManager.PlayAudioClip(menuMoveClip);
        lastSelected = currentSystem.currentSelectedGameObject;
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
