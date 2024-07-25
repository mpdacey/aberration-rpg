using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MusicManager))]
public class MusicEventController : MonoBehaviour
{
    public MusicScriptableObject battleMusic;
    public MusicScriptableObject fieldMusic;
    private MusicManager manager;

    private void Start()
    {
        manager = GetComponent<MusicManager>();
        StartFieldMusic();
    }

    private void OnEnable()
    {
        FormationSelector.FormationSelected += StartBattleMusic;
        CombatController.CombatVictory += StopBattleMusic;
        CombatController.GameoverEvent += Gameover;
    }

    private void OnDisable()
    {
        FormationSelector.FormationSelected -= StartBattleMusic;
        CombatController.CombatVictory -= StopBattleMusic;
        CombatController.GameoverEvent -= Gameover;
    }

    private void StartFieldMusic()
    {
        manager.PlayMusic(fieldMusic);
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
        StartCoroutine(FadeOutMusic(3));
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
