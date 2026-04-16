using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerLogCreator : MonoBehaviour
{
    private StreamWriter writer;
    private float startTime;
    private int creaturesDefeated;
    private int creaturesRecruited;
    private int equipmentCollected;

    private void OnEnable()
    {
        GameController.StartNewGameEvent += StartLog;
        GameController.ContinueGameEvent += StartLog;
        CombatController.GameoverEvent += () => EndLog(false);
        VictoryController.VictoryAchievedAction += () => EndLog(true);
        MonsterController.MonsterDefeated += (int value) => creaturesDefeated++;
        RecruitmentController.RecruitmentMade += () => creaturesRecruited++;
        EquipmentController.EquipmentUpdated += () => equipmentCollected++;
    }


    private void OnDisable()
    {
        GameController.StartNewGameEvent -= StartLog;
        GameController.ContinueGameEvent -= StartLog;
        CombatController.GameoverEvent -= () => EndLog(false);
        VictoryController.VictoryAchievedAction -= () => EndLog(true);
        MonsterController.MonsterDefeated -= (int value) => creaturesDefeated++;
        RecruitmentController.RecruitmentMade -= () => creaturesRecruited++;
        EquipmentController.EquipmentUpdated -= () => equipmentCollected++;
    }

    private void OnApplicationQuit()
    {
        EndLog(false);
    }

    private void StartLog()
    {
        writer = new StreamWriter($"{Application.dataPath}/Logs/{DateTime.Now.AddDays(0):yyyyMMddHHmmss}.log", true);
        writer.WriteLine($"Start Time: {DateTime.Now.AddDays(0):T}");

        startTime = Time.realtimeSinceStartup;
        creaturesDefeated = 0;
        creaturesRecruited = 0;
        equipmentCollected = 0;
    }

    private void EndLog(bool victoryAchieved)
    {
        if (writer == null)
            return;

        float playtime = Time.realtimeSinceStartup - startTime;

        writer.WriteLine($"End Time: {DateTime.Now.AddDays(0):T}");
        writer.WriteLine($"Play Duration: {TimeSpan.FromSeconds(playtime).ToString(@"mm\:ss")}");
        writer.WriteLine($"Did Escape: {(victoryAchieved ? "Yes": "No")}");
        writer.WriteLine($"Realms Clears: {GameController.CurrentLevel}");
        writer.WriteLine($"Creatures Defeated: {creaturesDefeated}");
        writer.WriteLine($"Creatures Recruited: {creaturesRecruited}");
        writer.WriteLine($"Equipment Collected: {equipmentCollected}");
        writer.Close();
        writer.Dispose();
        writer = null;
    }
}
