using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverController : MonoBehaviour
{
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
}
