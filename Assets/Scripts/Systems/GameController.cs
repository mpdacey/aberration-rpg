using UnityEngine;
using System;
using System.Collections;
using Cryptemental.SceneController;

public class GameController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember?, int> SetPartyMember;
    public static event Action ResetGameEvent;
    public static event Action ContinueGameEvent;
    public static event Action<Vector2> VolumesLoaded;

    public static int CurrentLevel
    {
        get => currentLevel;
    }
    private static int currentLevel = 0;

    DataManager dataManager;
    private bool hasLoadedVolume = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (ResetGameEvent != null)
            ResetGameEvent.Invoke();
        CallTitleScene();
    }

    private void OnEnable()
    {
        PartyController.PartyIsReady += SetPlayerUI;
        SceneController.CombatSceneLoaded += SetPlayerUI;
        SceneController.TitleSceneLoaded += LoadVolumes;
        GoalRiftController.GoalRiftEntered += IncrementCurrentLevel;
        GoalRiftController.GoalRiftEntered += AutoSaveGame;
        TitleManager.PlayButtonPressed += CallCombatScene;
        TitleManager.ContinueButtonPressed += ContinueGame;
        GameoverController.OnRetryEvent += ResetCombatScene;
        GameoverController.OnTitleEvent += ResetTitleScene;
        CombatController.GameoverEvent += ClearProgress;
        TitleVolumeUIManager.SavedMusicVolume += SaveMusicVolume;
        TitleVolumeUIManager.SavedSoundVolume += SaveSoundVolume;

        if (dataManager == null) dataManager = GetComponent<DataManager>();
        dataManager.SetFloorLevel += SetFloorLevel;
    }

    private void OnDisable()
    {
        PartyController.PartyIsReady -= SetPlayerUI;
        SceneController.CombatSceneLoaded -= SetPlayerUI;
        SceneController.TitleSceneLoaded -= LoadVolumes;
        GoalRiftController.GoalRiftEntered -= IncrementCurrentLevel;
        GoalRiftController.GoalRiftEntered -= AutoSaveGame;
        TitleManager.PlayButtonPressed -= CallCombatScene;
        TitleManager.ContinueButtonPressed -= ContinueGame;
        GameoverController.OnRetryEvent -= ResetCombatScene;
        GameoverController.OnTitleEvent -= ResetTitleScene;
        CombatController.GameoverEvent -= ClearProgress;
        TitleVolumeUIManager.SavedMusicVolume -= SaveMusicVolume;
        TitleVolumeUIManager.SavedSoundVolume -= SaveSoundVolume;
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
        StartCoroutine(ContinueInCombatScene());
    }

    IEnumerator ContinueInCombatScene()
    {
        yield return SceneController.LoadCombatScene();

        if (currentLevel > 0 && ContinueGameEvent != null)
            ContinueGameEvent.Invoke();
    }

    public void SetFloorLevel(int value) =>
        currentLevel = value;

    private void ClearProgress() =>
        dataManager.ClearProgress();

    private void CallCombatScene()
    {
        StartCoroutine(SceneController.LoadCombatScene());
    }

    private void CallTitleScene()
    {
        StartCoroutine(SceneController.LoadTitleScene());
    }

    private void SaveMusicVolume(float normalisedVolume)
    {
        if (!dataManager)
        {
            Debug.LogWarning("Couldn't save music volume because dataManager was null");
            return;
        }

        dataManager.SaveMusicVolume(normalisedVolume);
    }

    private void SaveSoundVolume(float normalisedVolume)
    {
        if (!dataManager)
        {
            Debug.LogWarning("Couldn't save sound volume because dataManager was null");
            return;
        }

        dataManager.SaveSoundVolume(normalisedVolume);
    }

    private void LoadVolumes()
    {
        if (hasLoadedVolume) return;
        hasLoadedVolume = true;

        if (!dataManager)
        {
            Debug.LogWarning("Couldn't save load volume because dataManager was null");
            return;
        }

        if (VolumesLoaded != null)
            VolumesLoaded.Invoke(dataManager.LoadVolumes(Vector2.zero));
    }
}
