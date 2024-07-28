using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SelectDefaultButton))]
public class TitleVolumeUIManager : MonoBehaviour
{
    public Button[] sliderButtons;
    private SelectDefaultButton defaultButtonSelector;
    private bool inSlider = false;
    private int buttonIndex = 0;

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
        sliderButtons[buttonIndex].GetComponentInChildren<Slider>().Select();
        inSlider = true;
    }

    private void Update()
    {
        CheckForSliderExit();
    }

    private void CheckForSliderExit()
    {
        if (!inSlider) return;
        if (Input.GetButtonDown("Cancel")) sliderButtons[buttonIndex].Select();
    }
}
