using System;
using UnityEngine;
using TMPro;

public class GameoverController : MonoBehaviour
{
    public static event Action OnTitleEvent;
    public static event Action OnRetryEvent;

    [SerializeField] Animation gameoverAnimation;
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

    private void PlayGameoverAnimation() =>
        gameoverAnimation.Play();

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
