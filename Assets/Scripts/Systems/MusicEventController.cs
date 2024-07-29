using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cryptemental.SceneController;

[RequireComponent(typeof(MusicManager))]
public class MusicEventController : MonoBehaviour
{
    [Header("Music Objects")]
    public MusicScriptableObject battleMusic;
    public MusicScriptableObject battleIntenseMusic;
    public MusicScriptableObject fieldMusic;
    public MusicScriptableObject titleMusic;
    public MusicScriptableObject gameoverMusic;

    private MusicManager manager;
    private double fieldTime = 0;
    private bool returnToField = false;

    private void Start()
    {
        manager = GetComponent<MusicManager>();
    }

    private void OnEnable()
    {
        SceneController.TitleSceneLoaded += StartTitleMusic;
        SceneController.CombatSceneLoaded += StartFieldMusic;
        FormationSelector.FormationThreatLevel += PickBattleMusic;
        CombatController.CombatVictory += StopBattleMusic;
        CombatController.GameoverEvent += Gameover;
    }

    private void OnDisable()
    {
        SceneController.TitleSceneLoaded -= StartTitleMusic;
        SceneController.CombatSceneLoaded -= StartFieldMusic;
        FormationSelector.FormationThreatLevel -= PickBattleMusic;
        CombatController.CombatVictory -= StopBattleMusic;
        CombatController.GameoverEvent -= Gameover;
    }

    private void StartTitleMusic()
    {
        manager.PlayMusic(titleMusic);
    }

    private void StartFieldMusic()
    {
        StartCoroutine(FadeOutMusic(0.8f, fieldMusic));
    }

    private void PickBattleMusic(int threatLevel)
    {
        int formationCount = FormationSelector.CurrentFormation.monsters.Length;
        int partyCount = 0;
        float partyHealthPercent = 0;
        float partyStaminaPercent = 0;
        for(int i = 0; i < PartyController.partyMembers.Length; i++)
        {
            if (PartyController.partyMembers[i].HasValue)
            {
                PartyController.PartyMember currentMember = PartyController.partyMembers[i].Value;
                partyHealthPercent += (float)currentMember.currentHP / currentMember.partyMemberBaseStats.combatantMaxHealth;
                partyStaminaPercent += (float)currentMember.currentSP / currentMember.partyMemberBaseStats.combatantMaxStamina;
                partyCount++;
            }
        }

        partyHealthPercent /= partyCount;
        partyStaminaPercent /= partyCount;

        bool lowThreat = threatLevel < 0 && partyHealthPercent < 0.3f;
        bool mediumThreat = threatLevel == 0 &&
            (((partyHealthPercent < 0.6f || partyStaminaPercent < 0.4f) && partyCount < 3) ||
            partyHealthPercent < 0.4f || partyStaminaPercent < 0.25f ||
            formationCount > partyCount);
        bool highThreat = threatLevel > 0 &&
            ((partyHealthPercent < 0.8f && partyCount < 3) ||
            ((partyHealthPercent < 0.7f || partyStaminaPercent < 0.6f) && partyCount < 4) ||
            partyHealthPercent < 0.5f || partyStaminaPercent < 0.4f ||
            formationCount > partyCount);
        bool lethalThreat = threatLevel > 1;
        bool isThreat = lowThreat || mediumThreat || highThreat || lethalThreat;

        if (isThreat)
            StartBattleMusic(battleIntenseMusic);
        else
            StartBattleMusic(battleMusic);
    }

    private void StartBattleMusic(MusicScriptableObject music)
    {
        fieldTime = manager.GetCurrentTime();
        StartCoroutine(FadeOutMusic(0.5f, music));
    }

    private void StopBattleMusic()
    {
        returnToField = true;
        StartCoroutine(FadeOutMusic(1.2f, fieldMusic));
    }

    private void Gameover()
    {
        StartCoroutine(FadeOutMusic(1.6f, gameoverMusic));
    }

    IEnumerator FadeOutMusic(float fadeoutTime, MusicScriptableObject nextTrack = null)
    {
        yield return manager.FadeMusicOut(fadeoutTime);

        if (!nextTrack)
            manager.StopMusic();

        if (returnToField)
        {
            manager.PlayMusic(nextTrack, resumeTime:fieldTime);
            returnToField = false;
            yield return manager.FadeInMusic(0.6f);
        }
        else
            manager.PlayMusic(nextTrack);
    }
}
