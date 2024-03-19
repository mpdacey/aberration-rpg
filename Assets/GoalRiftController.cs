using System;
using UnityEngine;

public class GoalRiftController : MonoBehaviour
{
    public static event Action GoalRiftEntered;

    public AudioClip playerWarpSFX;
    private Transform player;

    private void OnEnable()
    {
        LevelGenerator.GoalLocationFound += SetInitialPosition;
    }

    private void OnDisable()
    {
        LevelGenerator.GoalLocationFound -= SetInitialPosition;
    }

    private void Update()
    {
        if (player == null) player = GetComponent<RotateTowardsCamera>().player;
        else CheckForPlayer(player);
    }

    private void CheckForPlayer(Transform player)
    {
        if (Vector3.Distance(player.position, transform.position) < 1f && GoalRiftEntered != null)
            PerformAction();
    }

    virtual protected void PerformAction()
    {
        AudioManager.PlayAudioClip(playerWarpSFX);
        GoalRiftEntered.Invoke();
    }

    public void SetInitialPosition(Vector2 endLocation)
    {
        transform.position = new Vector3(endLocation.x*5, 0, endLocation.y * 5);
    }
}
