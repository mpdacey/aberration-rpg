using UnityEngine;
using UnityEngine.UI;
using Cryptemental.SceneController;

public class ManualInteractListener : MonoBehaviour
{
    public Sprite manualCloseSprite;
    public Sprite manualOpenSprite;
    public Image manualUIElement;
    private bool manualOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Manual"))
        {
            if (manualOpen)
            {
                StartCoroutine(SceneController.UnloadManualScene());
                manualUIElement.sprite = manualCloseSprite;
            }
            else
            {
                StartCoroutine(SceneController.LoadManualScene());
                manualUIElement.sprite = manualOpenSprite;
            }

            manualOpen = !manualOpen;
        }
    }
}
