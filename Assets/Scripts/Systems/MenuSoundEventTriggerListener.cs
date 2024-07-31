using UnityEngine;

public class MenuSoundEventTriggerListener : MonoBehaviour
{
    public void OnButtonMove()
    {
        Debug.Log("Hello");
    }

    public void OnButtonSubmit()
    {
        Debug.Log("Submit");
    }

    public void OnButtonCancel()
    {
        Debug.Log("Cancel");
    }
}
