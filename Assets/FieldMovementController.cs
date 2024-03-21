using System;
using System.Collections;
using UnityEngine;

public class FieldMovementController : MonoBehaviour
{
    public static event Action<Vector3> PlayerPositionChanged;
    public static event Action<Vector3> PlayerRotationChanged;
    public static event Action FieldMovementEvent;

    public static bool inBattle = false;
    public AudioClip playerMovementSFX;
    [SerializeField] Animator movementAnimator;

    private const string MOVE_FORWARD_STATE = "MoveForward";
    private const string BUMP_FORWARD_STATE = "MoveForwardBump";
    private const string TURN_LEFT_STATE = "TurnLeft";
    private const string TURN_RIGHT_STATE = "TurnRight";
    private const string TURN_AROUND_STATE = "TurnAround";
    private const string NULL_STATE = "Empty";

    private void Start()
    {
        movementAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GoalRiftController.GoalRiftEntered += ResetPlayerPosition;
    }

    private void OnDisable()
    {
        GoalRiftController.GoalRiftEntered -= ResetPlayerPosition;
    }

    private void ResetPlayerPosition()
    {
        StartCoroutine(WaitForAnimationToStop());
    }

    IEnumerator WaitForAnimationToStop()
    {
        while (!movementAnimator.GetCurrentAnimatorStateInfo(0).IsName(NULL_STATE))
            yield return new WaitForEndOfFrame();

        transform.position = Vector3.zero;
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
            if (Physics.Raycast(ray))
            {
                AudioManager.PlayAudioClip(playerMovementSFX, true);
                CallAnimation(MOVE_FORWARD_STATE);
                if (FieldMovementEvent != null)
                    FieldMovementEvent.Invoke();
                if (PlayerPositionChanged != null)
                    PlayerPositionChanged.Invoke(rayOrigin);
            }
            else
                CallAnimation(BUMP_FORWARD_STATE);
        }
        else if (horizontal > 0.5f)
        {
            CallAnimation(TURN_RIGHT_STATE);
            if (PlayerRotationChanged != null)
                PlayerRotationChanged.Invoke(new Vector3(0, 0, -90));
        }
        else if (horizontal < -0.5f)
        {
            CallAnimation(TURN_LEFT_STATE);
            if (PlayerRotationChanged != null)
                PlayerRotationChanged.Invoke(new Vector3(0, 0, 90));
        }
        else if (vertical < -0.5f)
        {
            CallAnimation(TURN_AROUND_STATE);
            if (PlayerRotationChanged != null)
                PlayerRotationChanged.Invoke(new Vector3(0, 0, 180));
        }
    }

    private void CallAnimation(string animationClipName)
    {
        movementAnimator.Play(animationClipName);
    }
}
