using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember?, int> SetPartyMember;
    public static event Action ResetGameEvent;

    public static int CurrentLevel
    {
        get => currentLevel;
    }
    private static int currentLevel = 0;

    SceneController sceneController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sceneController = GetComponent<SceneController>();
        CallTitleScene();
    }

    private void OnEnable()
    {
        PartyController.PartyIsReady += SetPlayerUI;
        GoalRiftController.GoalRiftEntered += IncrementCurrentLevel;
        TitleManager.PlayButtonPressed += CallCombatScene;
    }

    private void OnDisable()
    {
        PartyController.PartyIsReady -= SetPlayerUI;
        GoalRiftController.GoalRiftEntered -= IncrementCurrentLevel;
        TitleManager.PlayButtonPressed -= CallCombatScene;
    }

    private void SetPlayerUI()
    {
        for (int i = 0; i < PartyController.partyMembers.Length; i++)
        {
            if (SetPartyMember != null && PartyController.partyMembers[i].HasValue)
                SetPartyMember.Invoke(PartyController.partyMembers[i], i);
        }
    }

    private void IncrementCurrentLevel() =>
        currentLevel++;

    private void ResetGame()
    {
        currentLevel = 0;
        if (ResetGameEvent != null)
            ResetGameEvent.Invoke();
    }

    private void CallCombatScene()
    {
        ResetGame();
        StartCoroutine(sceneController.LoadCombatScene());
    }

    private void CallTitleScene()
    {
        StartCoroutine(sceneController.LoadTitleScene());
    }
}
