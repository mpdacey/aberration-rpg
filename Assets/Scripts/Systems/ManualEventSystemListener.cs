using UnityEngine;
using UnityEngine.EventSystems;
using Cryptemental.SceneController;

[RequireComponent(typeof(EventSystem))]
public class ManualEventSystemListener : MonoBehaviour
{
    public EventSystem combatEventSystem;

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
        combatEventSystem.enabled = false;
    }

    private void EnableEventSystem()
    {
        combatEventSystem.enabled = true;
    }
}
