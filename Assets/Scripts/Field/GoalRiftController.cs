using System;
using UnityEngine;

public class GoalRiftController : MonoBehaviour
{
    public static event Action GoalRiftEntered;

    public Transform player;
    public AudioClip playerWarpSFX;
    private bool portalIsActive;

    private void OnEnable()
    {
        LevelGenerator.GoalLocationFound += SetEndGoal;
    }

    private void OnDisable()
    {
        LevelGenerator.GoalLocationFound -= SetEndGoal;
    }

    private void FixedUpdate()
    {
        if(portalIsActive)
            CheckForPlayer(player);
    }

    private void CheckForPlayer(Transform player)
    {
        if (Vector3.Distance(player.position, transform.position) < 1f && GoalRiftEntered != null)
        {
            EnterRift();
        }
    }

    public void EnterRift()
    {
        portalIsActive = false;
        AudioManager.PlayAudioClip(playerWarpSFX);
        GoalRiftEntered.Invoke();
    }

    private void SetEndGoal(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
        portalIsActive = true;
    }
}
