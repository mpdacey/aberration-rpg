using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cryptemental.Manual
{
    using Cryptemental.SceneController;
    public class ManualInteractListener : MonoBehaviour
    {
        public static event Action CloseManualInput;

        public Sprite manualCloseSprite;
        public Sprite manualOpenSprite;
        public Image manualUIElement;
        private bool manualOpen = false;
        private bool hasClosedManual = false;

        private void OnEnable()
        {
            StateBehaviourCloseManualEvent.CloseManualEvent += CloseManual;
        }

        private void OnDisable()
        {
            StateBehaviourCloseManualEvent.CloseManualEvent -= CloseManual;
        }

        void Update()
        {
            if (Input.GetButtonDown("Manual"))
            {
                if (!manualOpen) OpenManual();
                else InitiateCloseManual();
            }
        }

        void OpenManual()
        {
            StartCoroutine(SceneController.LoadManualScene());
            manualUIElement.sprite = manualOpenSprite;
            manualOpen = true;
        }

        void InitiateCloseManual()
        {
            if (hasClosedManual) return;
            hasClosedManual = true;
            if (CloseManualInput != null)
                CloseManualInput.Invoke();
        }

        void CloseManual()
        {
            StartCoroutine(SceneController.UnloadManualScene());
            manualUIElement.sprite = manualCloseSprite;
            manualOpen = hasClosedManual = false;
        }
    }
}