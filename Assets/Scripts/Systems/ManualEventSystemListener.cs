using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cryptemental.SceneController;

[RequireComponent(typeof(EventSystem))]
public class ManualEventSystemListener : MonoBehaviour
{
    public EventSystem combatEventSystem;
    private Selectable lastSelectable = null;

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
        if(combatEventSystem.currentSelectedGameObject != null)
            lastSelectable = combatEventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        combatEventSystem.enabled = false;
    }

    private void EnableEventSystem()
    {
        combatEventSystem.enabled = true;
        if(lastSelectable != null)
            lastSelectable.Select();
    }
}
