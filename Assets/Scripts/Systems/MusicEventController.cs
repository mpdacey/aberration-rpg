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
        CombatController.CombatVictory += StopMusic;
        CombatController.GameoverEvent += Gameover;
    }

    private void OnDisable()
    {
        FormationSelector.FormationSelected -= StartBattleMusic;
        CombatController.CombatVictory -= StopMusic;
        CombatController.GameoverEvent -= Gameover;
    }

    private void StartFieldMusic()
    {
        manager.PlayMusic(fieldMusic);
    }

    private void StartBattleMusic()
    {
        manager.PlayMusic(battleMusic);
    }

    private void StopMusic()
    {
        StartCoroutine(FadeOutMusic(fieldMusic));
    }

    private void Gameover()
    {
        StartCoroutine(FadeOutMusic());
    }

    IEnumerator FadeOutMusic(MusicScriptableObject nextTrack = null)
    {
        yield return manager.FadeMusicOut(1.2f);

        if (nextTrack)
            manager.PlayMusic(nextTrack);
        else
            manager.StopMusic();
    }
}
