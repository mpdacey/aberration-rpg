using System;
using System.Collections;
using UnityEngine;

public class FieldMovementController : MonoBehaviour
{
    public static event Action<Vector3> PlayerPositionChanged;
    public static event Action<Vector3> PlayerRotationChanged;
    public static event Action TreasureFound;
    public static event Action<Animator> TreasureFoundAnimator;

    public static bool lockedInPlace = false;
    public AudioClip playerMovementSFX;
    [SerializeField] Animator movementAnimator;
    private bool onTitle = false;

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
        GameController.ResetGameEvent += ResetPlayerPosition;
        LevelGenerator.FinishedLevelGeneration += SetPlayerOrientation;
        SceneController.TitleSceneLoaded += OnTitle;
        SceneController.CombatSceneLoaded += OnCombat;
    }

    private void OnDisable()
    {
        GoalRiftController.GoalRiftEntered -= ResetPlayerPosition;
        GameController.ResetGameEvent -= ResetPlayerPosition;
        LevelGenerator.FinishedLevelGeneration -= SetPlayerOrientation;
        SceneController.TitleSceneLoaded -= OnTitle;
        SceneController.CombatSceneLoaded -= OnCombat;
    }

    private void OnTitle()
    {
        onTitle = true;
        lockedInPlace = true;
    }

    private void OnCombat()
    {
        onTitle = false;
        lockedInPlace = false;
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

    private void SetPlayerOrientation()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 rayOrigin = transform.position + transform.rotation * Vector3.forward * 5;
            Ray ray = new Ray(rayOrigin, Vector3.down * 3);
            if (Physics.Raycast(ray))
            {
                Vector3 nextCell = rayOrigin + transform.rotation * Vector3.forward * 5;
                rayOrigin = nextCell + transform.rotation * Vector3.left * 5;

                ray.origin = rayOrigin;
                if (Physics.Raycast(ray)) break;

                rayOrigin = nextCell + transform.rotation * Vector3.forward * 5;
                ray.origin = rayOrigin;
                if (Physics.Raycast(ray)) break;


                rayOrigin = nextCell + transform.rotation * Vector3.right * 5;
                ray.origin = rayOrigin;
                if (Physics.Raycast(ray)) break;
            }

            transform.Rotate(0, 90, 0);
            if (PlayerRotationChanged != null)
                PlayerRotationChanged.Invoke(new Vector3(0, 0, -90));
        }
    }

    void Update()
    {
        if (lockedInPlace || onTitle || !movementAnimator.GetCurrentAnimatorStateInfo(0).IsName(NULL_STATE))
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical > 0.5f)
        {
            Vector3 rayOrigin = transform.position + transform.rotation * Vector3.forward * 5;
            RaycastHit raycastInfo;
            Ray ray = new(rayOrigin, Vector3.down * 6);
            if (Physics.Raycast(ray, out raycastInfo))
            {
                if (raycastInfo.collider.tag == "Treasure") TreasureHandling(raycastInfo.transform);
                else
                {
                    if (playerMovementSFX != null)
                        AudioManager.PlayAudioClip(playerMovementSFX, true);
                    CallAnimation(MOVE_FORWARD_STATE);
                    if (PlayerPositionChanged != null)
                        PlayerPositionChanged.Invoke(rayOrigin);
                }
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

    private void TreasureHandling(Transform treasureRift)
    {
        if (TreasureFound != null)
            TreasureFound.Invoke();
        if (TreasureFoundAnimator != null && treasureRift.TryGetComponent<Animator>(out var animator))
            TreasureFoundAnimator.Invoke(animator);
    }
}
