using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public static event Action<PartyController.PartyMember?, int> SetPartyMember;

    public static int CurrentLevel
    {
        get => currentLevel;
    }
    private static int currentLevel = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        PartyController.PartyIsReady += SetPlayerUI;
        GoalRiftController.GoalRiftEntered += IncrementCurrentLevel;
    }

    private void OnDisable()
    {
        PartyController.PartyIsReady -= SetPlayerUI;
        GoalRiftController.GoalRiftEntered -= IncrementCurrentLevel;
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
}
