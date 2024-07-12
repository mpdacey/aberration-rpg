using UnityEngine;
namespace Cryptemental.Manual
{
    [RequireComponent(typeof(ManualUIController))]
    public class ManualController : MonoBehaviour
    {
        [System.Serializable]
        public struct ManualPage
        {
            public string name;
            public Sprite content;
        }

        public ManualPage[] manualPages;
        public GameObject pageMarkerPrefab;
        private ManualUIController uiController;

        private void OnValidate()
        {
            if(uiController == null)
                uiController = GetComponent<ManualUIController>();
            UpdatePages();
        }

        private void UpdatePages()
        {
            for (int i = uiController.pageMarkerColumn.childCount; i > manualPages.Length && i > 0; i--)
            {
                // Destroy on validate code: https://forum.unity.com/threads/onvalidate-and-destroying-objects.258782/#post-1710165
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(uiController.pageMarkerColumn.GetChild(i - 1).gameObject);
                };
            }

            for (int i = 0; i < manualPages.Length; i++)
            {
                if (i >= uiController.pageMarkerColumn.childCount)
                    Instantiate(pageMarkerPrefab, uiController.pageMarkerColumn);

                uiController.UpdatePageMarker(i, manualPages[i]);
            }
        }
    }
}
