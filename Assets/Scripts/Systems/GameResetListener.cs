using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameResetListener : MonoBehaviour
{
    public UnityEvent GameResetPressed;

    public void Update()
    {
        if (Input.GetButtonDown("Reset"))
            GameResetPressed.Invoke();
    }
}
