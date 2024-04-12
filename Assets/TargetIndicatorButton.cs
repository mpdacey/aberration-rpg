using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicatorButton : Button
{
    private Image[] childImages;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        childImages = transform.GetChild(0).GetComponentsInChildren<Image>();
        StopAllCoroutines();

        switch (state)
        {
            case SelectionState.Normal:
                StartCoroutine(LerpToNewColour(colors.normalColor, instant ? 0 : colors.fadeDuration));
                break;
            case SelectionState.Selected:
                StartCoroutine(LerpToNewColour(colors.selectedColor, instant ? 0 : colors.fadeDuration));
                break;
            case SelectionState.Pressed:
                StartCoroutine(LerpToNewColour(colors.pressedColor, instant ? 0 : colors.fadeDuration));
                break;
        }
    }

    private IEnumerator LerpToNewColour(Color newColor, float fadeDuration)
    {
        foreach (var image in childImages) image.material.color = Color.white;
        Debug.Log(childImages.Length);
        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            foreach (var image in childImages) image.color = Color.Lerp(image.color, newColor, t / fadeDuration);
            yield return new WaitForEndOfFrame();
        }
        foreach (var image in childImages) image.color = newColor;
    }
}
