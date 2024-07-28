using System;
using UnityEngine;
using UnityEngine.UI;
using Cryptemental.Data;

public class TitleManager : MonoBehaviour
{
    public static event Action PlayButtonPressed;
    public static event Action ContinueButtonPressed;

    public Button continueButton;

    private void OnEnable()
    {
        bool hasSave = PlayerPrefs.HasKey(PlayerPrefsKeys.FLOOR_KEY) && PlayerPrefs.GetInt(PlayerPrefsKeys.FLOOR_KEY) > 0;
        continueButton.enabled = hasSave;
    }

    public void OnPlayButton()
    {
        if (PlayButtonPressed != null)
            PlayButtonPressed.Invoke();
    }

    public void OnContinueButton()
    {
        if (ContinueButtonPressed != null)
            ContinueButtonPressed.Invoke();
    }
}
