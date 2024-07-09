using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDefaultButton : MonoBehaviour
{
    [SerializeField] Button defaultButton;

    private void OnEnable()
    {
        defaultButton.Select();
    }

    private void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            defaultButton.Select();
    }

}
