using System;
using UnityEngine;
using Cryptemental.Data;

public class TitleManager : MonoBehaviour
{
    public static event Action PlayButtonPressed;
    public static event Action ContinueButtonPressed;

    public GameObject continueButton;

    private void OnEnable()
    {
        bool hasSave = PlayerPrefs.HasKey(PlayerPrefsKeys.FLOOR_KEY) && PlayerPrefs.GetInt(PlayerPrefsKeys.FLOOR_KEY) > 0;
        continueButton.SetActive(hasSave);
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
