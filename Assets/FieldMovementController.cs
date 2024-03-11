using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMovementController : MonoBehaviour
{
    public bool inBattle = false;
    [SerializeField] Animator movementAnimator;
    private float currentAnimationTimer = 0;

    private const string MOVE_FORWARD_STATE = "MoveForward";
    private const string TURN_LEFT_STATE = "TurnLeft";
    private const string TURN_RIGHT_STATE = "TurnRight";
    private const string TURN_AROUND_STATE = "TurnAround";
    private const string NULL_STATE = "Empty";

    private void Start()
    {
        movementAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (inBattle || !movementAnimator.GetCurrentAnimatorStateInfo(0).IsName(NULL_STATE))
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical > 0.5f)
        {
            Vector3 rayOrigin = transform.position + transform.rotation * Vector3.forward * 5;
            Ray ray = new Ray(rayOrigin, Vector3.down * 3);
            if(Physics.Raycast(ray))
                CallAnimation(MOVE_FORWARD_STATE);
        }
        else if (horizontal > 0.5f) CallAnimation(TURN_RIGHT_STATE);
        else if (horizontal < -0.5f) CallAnimation(TURN_LEFT_STATE);
        else if (vertical < -0.5f) CallAnimation(TURN_AROUND_STATE);
    }

    private void CallAnimation(string animationClipName)
    {
        movementAnimator.Play(animationClipName);
    }
}
