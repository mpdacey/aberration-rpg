using System;
using UnityEngine;
using UnityEngine.UI;
using Cryptemental.Data;

public class TitleManager : MonoBehaviour
{
    public static event Action PlayButtonPressed;
    public static event Action ContinueButtonPressed;

    public Button continueButton;
    public TitleVolumeUIManager settingsUIManager;
    private Animator animator;

    private const string MAIN_ANIM_KEY = "TitleMain";
    private const string SETTINGS_ANIM_KEY = "TitleMusic";
    private const string EMPTY_ANIM_KEY = "Empty";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        bool hasSave = PlayerPrefs.HasKey(PlayerPrefsKeys.FLOOR_KEY) && PlayerPrefs.GetInt(PlayerPrefsKeys.FLOOR_KEY) > 0;
        continueButton.enabled = hasSave;

        settingsUIManager.ReturnToMainMenu += OnReturnButton;
    }

    private void OnDisable()
    {
        settingsUIManager.ReturnToMainMenu -= OnReturnButton;
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

    public void OnSettingsButton()
    {
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName(SETTINGS_ANIM_KEY))
            animator.Play(SETTINGS_ANIM_KEY);
    }

    public void OnReturnButton()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(MAIN_ANIM_KEY))
            animator.Play(MAIN_ANIM_KEY);
    }
}
