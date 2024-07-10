using UnityEngine;

public class ManualUIController : MonoBehaviour
{
    private Animator animator;
    private AudioSource sfx;

    void Start()
    {
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
}
