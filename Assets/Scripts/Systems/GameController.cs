using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember?, int> SetPartyMember;
    public static event Action ResetGameEvent;
    public static event Action ContinueGameEvent;

    public static int CurrentLevel
    {
        get => currentLevel;
    }
    private static int currentLevel = 0;

    SceneController sceneController;
    DataManager dataManager;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sceneController = GetComponent<SceneController>();
        if (ResetGameEvent != null)
            ResetGameEvent.Invoke();
        CallTitleScene();
    }

    private void OnEnable()
    {
        PartyController.PartyIsReady += SetPlayerUI;
        SceneController.CombatSceneLoaded += SetPlayerUI;
        GoalRiftController.GoalRiftEntered += IncrementCurrentLevel;
        GoalRiftController.GoalRiftEntered += AutoSaveGame;
        TitleManager.PlayButtonPressed += CallCombatScene;
        TitleManager.ContinueButtonPressed += ContinueGame;
        GameoverController.OnRetryEvent += ResetCombatScene;
        GameoverController.OnTitleEvent += ResetTitleScene;
        CombatController.GameoverEvent += ClearProgress;

        if(dataManager == null) dataManager = GetComponent<DataManager>();
        dataManager.SetFloorLevel += SetFloorLevel;
    }

    private void OnDisable()
    {
        PartyController.PartyIsReady -= SetPlayerUI;
        SceneController.CombatSceneLoaded -= SetPlayerUI;
        GoalRiftController.GoalRiftEntered -= IncrementCurrentLevel;
        GoalRiftController.GoalRiftEntered -= AutoSaveGame;
        TitleManager.PlayButtonPressed -= CallCombatScene;
        TitleManager.ContinueButtonPressed -= ContinueGame;
        GameoverController.OnRetryEvent -= ResetCombatScene;
        GameoverController.OnTitleEvent -= ResetTitleScene;
        CombatController.GameoverEvent -= ClearProgress;
        dataManager.SetFloorLevel -= SetFloorLevel;
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

    private void ResetCombatScene()
    {
        ResetGame();
        CallCombatScene();
    }

    private void ResetTitleScene()
    {
        ResetGame();
        CallTitleScene();
    }

    private void ResetGame()
    {
        currentLevel = 0;
        if (ResetGameEvent != null)
            ResetGameEvent.Invoke();
    }

    private void AutoSaveGame() =>
        dataManager.SaveProgress();

    private void ContinueGame()
    {
        dataManager.LoadProgress();
        if (ContinueGameEvent != null && currentLevel > 0)
            ContinueGameEvent.Invoke();
        CallCombatScene();
    }

    public void SetFloorLevel(int value) =>
        currentLevel = value;

    private void ClearProgress() =>
        dataManager.ClearProgress();

    private void CallCombatScene()
    {
        StartCoroutine(sceneController.LoadCombatScene());
    }

    private void CallTitleScene()
    {
        StartCoroutine(sceneController.LoadTitleScene());
    }
}
