using UnityEngine;

public class RotateTowardPlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.rotation = player.rotation;
    }
}
