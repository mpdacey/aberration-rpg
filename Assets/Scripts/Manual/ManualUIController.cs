using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cryptemental.Manual
{
    public class ManualUIController : MonoBehaviour
    {
        public Transform pageMarkerColumn;

        private Animator animator;
        private AudioSource sfx;

        void Start()
        {
            animator = GetComponent<Animator>();
            sfx = GetComponent<AudioSource>();
        }

        void Update()
        {
            float input = Input.GetAxisRaw("Horizontal");
            if (input == 0 || !animator.GetCurrentAnimatorStateInfo(0).IsName("Empty")) return;

            if (input > 0)
            {
                animator.Play("ManualLeft");
                sfx.PlayOneShot(sfx.clip);
            }
            else
            {
                animator.Play("ManualRight");
                sfx.PlayOneShot(sfx.clip);
            }
        }

        public void UpdatePageMarker(int siblingIndex, ManualController.ManualPage manualPage)
        {
            Transform child = pageMarkerColumn.GetChild(siblingIndex);
            child.GetComponentInChildren<TMP_Text>().text = manualPage.name;
            child.GetComponentInChildren<Image>().sprite = manualPage.content;
        }
    }
}