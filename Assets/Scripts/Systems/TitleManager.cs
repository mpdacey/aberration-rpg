using System;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public static event Action PlayButtonPressed;

    public void OnPlayButton()
    {
        if (PlayButtonPressed != null)
            PlayButtonPressed.Invoke();
    }
}
