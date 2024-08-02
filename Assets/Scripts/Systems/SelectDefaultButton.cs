using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDefaultButton : MonoBehaviour
{
    public Button defaultButton;

    private void OnEnable()
    {
        if (!defaultButton) return;
        defaultButton.Select();
    }

    private void Update()
    {
        if (!defaultButton) return;

        if (EventSystem.current == null || EventSystem.current.gameObject.scene.buildIndex != gameObject.scene.buildIndex) return;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            defaultButton.Select();
    }
}
