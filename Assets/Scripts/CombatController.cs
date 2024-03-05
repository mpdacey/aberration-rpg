using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public bool isPlayerTurn = true;
    public MonsterController[] monsters;
    public FormationScriptableObject formation;

    private void Start()
    {
        SetupCombat();
    }

    public void SetupCombat()
    {
        foreach (var monster in monsters)
            monster.GetComponent<SpriteRenderer>().enabled = false;

        switch (formation.monsters.Length)
        {
            case 3:
                for(int i = 0; i < 3; i++)
                {
                    monsters[i].CombatantStats = formation.monsters[i];
                    monsters[i].GetComponent<SpriteRenderer>().enabled = true;
                    monsters[i].transform.position = Vector3.left * (4.5f - 4.5f * i);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    monsters[i * 2].CombatantStats = formation.monsters[i];
                    monsters[i * 2].GetComponent<SpriteRenderer>().enabled = true;
                    monsters[i * 2].transform.position = Vector3.left * (3 - 6 * i);
                }
                break;
            case 1:
                monsters[1].CombatantStats = formation.monsters[0];
                monsters[1].GetComponent<SpriteRenderer>().enabled = true;
                break;
            default:
                Debug.LogError($"Invalid number of monsters in foundation: {formation.name}");
                return;
        }


    }
}
