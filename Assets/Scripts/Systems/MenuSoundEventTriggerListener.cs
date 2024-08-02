using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Cryptemental.Audio;

public class MenuSoundEventTriggerListener : MonoBehaviour
{
    public AudioClip menuMoveClip;
    public AudioClip menuConfirmClip;
    public AudioClip menuCancelClip;

    private GameObject lastSelected = null;

    public static bool isInspecting = false;

    public void OnButtonMove()
    {
        EventSystem currentSystem = EventSystem.current;
        if (currentSystem.currentSelectedGameObject == lastSelected) return;

        AudioManager.PlayAudioClip(menuMoveClip);
        StartCoroutine(SetLastSelected());
    }

    public void OnButtonSubmit()
    {
        if (isInspecting) return;
        AudioManager.PlayAudioClip(menuConfirmClip);
        StartCoroutine(SetLastSelected());
    }

    public void OnButtonCancel()
    {
        AudioManager.PlayAudioClip(menuCancelClip);
        StartCoroutine(SetLastSelected());
    }

    public void OnSliderValueChange()
    {
        AudioManager.PlayAudioClip(menuMoveClip);
    }

    IEnumerator SetLastSelected()
    {
        yield return new WaitForEndOfFrame();

        EventSystem currentSystem = EventSystem.current;
        if (currentSystem && currentSystem.currentSelectedGameObject != lastSelected)
            lastSelected = currentSystem.currentSelectedGameObject;
    }
}
