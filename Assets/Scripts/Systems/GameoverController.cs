using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameoverController : MonoBehaviour
{
    public static event Action OnTitleEvent;
    public static event Action OnRetryEvent;

    [SerializeField] Animator gameoverAnimator;
    public TextMeshProUGUI gameoverFloorCounter;

    private void OnEnable()
    {
        GoalRiftController.GoalRiftEntered += UpdateFloorCounter;
        CombatController.GameoverEvent += PlayGameoverAnimation;
    }

    private void OnDisable()
    {
        GoalRiftController.GoalRiftEntered -= UpdateFloorCounter;
        CombatController.GameoverEvent -= PlayGameoverAnimation;
    }

    private void UpdateFloorCounter() =>
        gameoverFloorCounter.text = $"{GameController.CurrentLevel + 1}";

    private void PlayGameoverAnimation()
    {
        gameoverAnimator.Play("Gameover");
        StartCoroutine(AllowSkipingDuringAnimation());
    }

    IEnumerator AllowSkipingDuringAnimation()
    {
        do
        {
            if (Input.anyKeyDown)
                SkipAnimation();
            yield return null;
        } while (gameoverAnimator.GetCurrentAnimatorStateInfo(0).IsName("Gameover") && gameoverAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
    }

    private void SkipAnimation() =>
        gameoverAnimator.Play("Gameover", 0, 1);

    public void OnRetryButtonPress()
    {
        if (OnRetryEvent != null)
            OnRetryEvent.Invoke();
    }

    public void OnTitleButtonPress()
    {
        if (OnTitleEvent != null)
            OnTitleEvent.Invoke();
    }
}
