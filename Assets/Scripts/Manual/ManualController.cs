using UnityEngine;
namespace Cryptemental.Manual
{
    using Cryptemental.Audio;

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
        public AudioClip pageTurnSFX;
        private ManualUIController uiController;
        private Animator animator;
        private int currentIndex = 0;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            ManualPageOnSelect.OnManualPageSelect += FlipToPage;
            ManualInteractListener.CloseManualInput += PlayCloseManual;
            uiController.SetPageContent(manualPages[0].content);
        }

        private void OnDisable()
        {
            ManualPageOnSelect.OnManualPageSelect -= FlipToPage;
            ManualInteractListener.CloseManualInput -= PlayCloseManual;
        }

        private void FlipToPage(int index)
        {
            if (index == currentIndex) return;

            if(index > currentIndex)
            {
                animator.Play("ManualRight", -1, 0f);
                if (pageTurnSFX != null)
                    AudioManager.PlayAudioClip(pageTurnSFX, true);
            }
            else
            {
                animator.Play("ManualLeft", -1, 0f);
                if (pageTurnSFX != null)
                    AudioManager.PlayAudioClip(pageTurnSFX, true);
            }

            uiController.SetPageContent(manualPages[index].content);
            currentIndex = index;
        }

        private void PlayCloseManual()
        {
            animator.Play("ManualClose");
        }

#if UNITY_EDITOR
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
                    UnityEditor.PrefabUtility.InstantiatePrefab(pageMarkerPrefab, uiController.pageMarkerColumn);

                uiController.UpdatePageMarker(i, manualPages[i]);
            }

            uiController.SetDefaultButton();
        }
#endif
    }
}
