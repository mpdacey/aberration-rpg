using UnityEngine;

public class RotateTowardPlayer : MonoBehaviour
{
    public Transform player;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        transform.rotation = player.rotation;
    }
}
