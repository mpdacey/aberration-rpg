using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class ManualEventSystemListener : MonoBehaviour
{
    private void OnEnable()
    {
        SceneController.ManualSceneLoaded += DisableEventSystem;
        SceneController.ManualSceneUnloaded += EnableEventSystem;
    }

    private void OnDisable()
    {
        SceneController.ManualSceneLoaded -= DisableEventSystem;
        SceneController.ManualSceneUnloaded -= EnableEventSystem;
    }

    private void DisableEventSystem()
    {
        EventSystem.current.enabled = false;
    }

    private void EnableEventSystem()
    {
        EventSystem.current.enabled = true;
    }
}
