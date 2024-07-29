using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SelectDefaultButton))]
public class TitleVolumeUIManager : MonoBehaviour
{
    public static event Action<float> SavedMusicVolume;
    public static event Action<float> SavedSoundVolume;

    public Button[] sliderButtons;
    private SelectDefaultButton defaultButtonSelector;
    private bool inSlider = false;
    private bool ignoreFirstInput = false;
    private int buttonIndex = 0;
    private int previousSliderState = 0;
    private Slider currentSlider = null;

    private const int MUSIC_SLIDER_INDEX = 0;
    private const int SOUND_SLIDER_INDEX = 1;

    void Start()
    {
        defaultButtonSelector = GetComponent<SelectDefaultButton>();
    }

    public void OnSelectButton(int index)
    {
        if (!defaultButtonSelector) return;
        buttonIndex = index;
        defaultButtonSelector.defaultButton = sliderButtons[buttonIndex];
        inSlider = false;
    }

    public void SelectSlider()
    {
        currentSlider = sliderButtons[buttonIndex].GetComponentInChildren<Slider>();
        previousSliderState = (int)currentSlider.value;
        currentSlider.Select();
        inSlider = true;
        ignoreFirstInput = true;
    }

    private void Update()
    {
        CheckForSliderExit();
    }

    private void CheckForSliderExit()
    {
        if (!inSlider) return;
        if (ignoreFirstInput)
        {
            ignoreFirstInput = false;
            return;
        }

        if (Input.GetButtonDown("Submit")) ConfirmChanges();
        else if (Input.GetButtonDown("Cancel")) DiscardChanges();
    }

    private void ConfirmChanges()
    {
        switch (buttonIndex)
        {
            case MUSIC_SLIDER_INDEX:
                if (SavedMusicVolume != null)
                    SavedMusicVolume.Invoke(currentSlider.value / currentSlider.maxValue);
                break;
            case SOUND_SLIDER_INDEX:
                if (SavedSoundVolume != null)
                    SavedSoundVolume.Invoke(currentSlider.value / currentSlider.maxValue);
                break;
        }
        ReturnToParentButton();
    }

    private void DiscardChanges()
    {
        currentSlider.value = previousSliderState;
        ReturnToParentButton();
    }

    private void ReturnToParentButton()
    {
        sliderButtons[buttonIndex].Select();
        currentSlider = null;
    }
}
