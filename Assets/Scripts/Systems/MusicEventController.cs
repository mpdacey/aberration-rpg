using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MusicManager))]
public class MusicEventController : MonoBehaviour
{
    public MusicScriptableObject battleMusic;
    private MusicManager manager;

    private void Start()
    {
        manager = GetComponent<MusicManager>();
    }

    private void OnEnable()
    {
        FormationSelector.FormationSelected += StartBattleMusic;
        CombatController.CombatVictory += StopMusic;
        CombatController.GameoverEvent += StopMusic;
    }

    private void OnDisable()
    {
        FormationSelector.FormationSelected -= StartBattleMusic;
        CombatController.CombatVictory -= StopMusic;
        CombatController.GameoverEvent -= StopMusic;
    }

    private void StartBattleMusic()
    {
        manager.PlayMusic(battleMusic);
    }

    private void StopMusic()
    {
        manager.StopMusic();
    }
}
