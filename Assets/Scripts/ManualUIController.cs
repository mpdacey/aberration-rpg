using UnityEngine;

public class ManualUIController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input == 0 || !animator.GetCurrentAnimatorStateInfo(0).IsName("Empty")) return;

        if (input > 0)
        {
            animator.Play("ManualLeft");
        }
        else
        {
            animator.Play("ManualRight");
        }
    }
}
