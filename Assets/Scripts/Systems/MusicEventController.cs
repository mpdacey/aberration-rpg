using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cryptemental.SceneController;

[RequireComponent(typeof(MusicManager))]
public class MusicEventController : MonoBehaviour
{
    public MusicScriptableObject battleMusic;
    public MusicScriptableObject fieldMusic;
    public MusicScriptableObject titleMusic;
    public MusicScriptableObject gameoverMusic;
    private MusicManager manager;

    private void Start()
    {
        manager = GetComponent<MusicManager>();
    }

    private void OnEnable()
    {
        SceneController.TitleSceneLoaded += StartTitleMusic;
        SceneController.CombatSceneLoaded += StartFieldMusic;
        FormationSelector.FormationSelected += StartBattleMusic;
        CombatController.CombatVictory += StopBattleMusic;
        CombatController.GameoverEvent += Gameover;
    }

    private void OnDisable()
    {
        SceneController.TitleSceneLoaded -= StartTitleMusic;
        SceneController.CombatSceneLoaded -= StartFieldMusic;
        FormationSelector.FormationSelected -= StartBattleMusic;
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

    private void StartBattleMusic()
    {
        StartCoroutine(FadeOutMusic(0.5f, battleMusic));
    }

    private void StopBattleMusic()
    {
        StartCoroutine(FadeOutMusic(1.2f, fieldMusic));
    }

    private void Gameover()
    {
        StartCoroutine(FadeOutMusic(1.6f, gameoverMusic));
    }

    IEnumerator FadeOutMusic(float fadeoutTime, MusicScriptableObject nextTrack = null)
    {
        yield return manager.FadeMusicOut(fadeoutTime);

        if (nextTrack)
            manager.PlayMusic(nextTrack);
        else
            manager.StopMusic();
    }
}
