using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static event Action<int> FormationCount;
    public event Action<int> CurrentPartyTurn;
    public event Action ShowAttackMenu;
    public event Action<bool[]> ShowTargetIndicator;

    struct PlayerAction
    {
        public ActionState actionType;
        public int target;
        public AttackObject attack;
    }

    enum BattleState
    {
        Initializing,
        PlayerPhase,
        EnemyPhase,
        Victory,
        Defeat
    }

    [Serializable]
    public enum ActionState
    {
        None,
        Attack,
        Skill,
        Item,
        Guard,
        Flee,
        Switch,
        Analyze,
        Confirm
    }

    public MonsterController[] monsters;
    public FormationScriptableObject formation;
    public ActionState actionState = ActionState.None;
    [SerializeField] private BattleState currentState;
    private PartyController.PartyMember currentMember;
    private int selectedMonster = 0;
    private bool[] monstersAlive;
    private PlayerAction[] playerActions = new PlayerAction[4];

    private void OnEnable()
    {
        MonsterController.MonsterDefeated += MonsterDeathHandling;
        SetupCombat();
    }

    private void OnDisable()
    {
        MonsterController.MonsterDefeated -= MonsterDeathHandling;
    }

    public void SetupCombat()
    {
        currentState = BattleState.Initializing;

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
                monstersAlive = new bool[3];
                monstersAlive[0] = monstersAlive[1] = monstersAlive[2] = true;
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    monsters[i * 2].CombatantStats = formation.monsters[i];
                    monsters[i * 2].GetComponent<SpriteRenderer>().enabled = true;
                    monsters[i * 2].transform.position = Vector3.left * (3 - 6 * i);
                }
                monstersAlive = new bool[2];
                monstersAlive[0] = monstersAlive[1] = true;
                break;
            case 1:
                monsters[1].CombatantStats = formation.monsters[0];
                monsters[1].GetComponent<SpriteRenderer>().enabled = true;
                monstersAlive = new bool[1];
                monstersAlive[0] = true;
                break;
            default:
                Debug.LogError($"Invalid number of monsters in foundation: {formation.name}");
                return;
        }

        StartCoroutine(BeginBattle());
    }

    public void SetActionState(int value) =>
        actionState = (ActionState)Mathf.Clamp(value, 0, Enum.GetValues(typeof(ActionState)).Length);

    public void SelectMonster(int value) =>
        selectedMonster = Mathf.Clamp(value, 0, 3);

    private void MonsterDeathHandling(int monsterID)
    {
        if(monsterID > monsters.Length)
        {
            Debug.LogError($"Monster ID {monsterID} is out of the monster array scope.");
            return;
        }

        monstersAlive[monsterID] = false;
        //monsters[monsterID].GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator BeginBattle()
    {
        yield return null;

        bool playerGoesFirst = UnityEngine.Random.value < 0.8f;

        if (playerGoesFirst)
            StartCoroutine(PlayerPhase());
        else
            StartCoroutine(EnemyPhase());
    }

    IEnumerator PlayerPhase()
    {
        currentState = BattleState.PlayerPhase;
        bool actionChosen = false;
        int lastValidIndex = 0;

        for(int currentPlayerIndex = 0; currentPlayerIndex < 4; currentPlayerIndex++)
        {
            if (!PartyController.partyMembers[currentPlayerIndex].HasValue)
                continue;

            currentMember = PartyController.partyMembers[currentPlayerIndex].Value;
            if (CurrentPartyTurn != null)
                CurrentPartyTurn.Invoke(currentPlayerIndex);

            actionState = ActionState.None;
            actionChosen = false;
            while (!actionChosen)
            {
                if (ShowAttackMenu != null)
                    ShowAttackMenu.Invoke();
                while(actionState == ActionState.None) yield return new WaitForEndOfFrame();

                switch (actionState)
                {
                    case ActionState.Attack:
                        // Select Enemy
                        if (ShowTargetIndicator != null)
                            ShowTargetIndicator.Invoke(monstersAlive);

                        while(actionState == ActionState.Attack)
                        {
                            if (Input.GetButtonDown("Cancel"))
                                actionState = ActionState.None;

                            yield return new WaitForEndOfFrame();
                        }

                        if(actionState == ActionState.Confirm)
                        {
                            PlayerAction playerAction = new()
                            {
                                actionType = ActionState.Attack,
                                target = selectedMonster,
                                attack = AttackHandler.GenerateNormalAttack(currentMember.partyMemberBaseStats)
                            };

                            playerActions[currentPlayerIndex] = playerAction;
                        }
                        break;
                    case ActionState.Skill:
                        // Select Skill
                        // Select Enemy
                        break;
                    case ActionState.Guard:
                        break;
                }

                actionChosen = actionState == ActionState.Confirm;
            }

            lastValidIndex = currentPlayerIndex;
        }

        CurrentPartyTurn.Invoke(-1);

        // Perform actions
        yield return PlayerActionExecution();

        StartCoroutine(EnemyPhase());
    }

    IEnumerator PlayerActionExecution()
    {
        for(int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].actionType == ActionState.None)
                continue;

            switch (playerActions[i].actionType)
            {
                case ActionState.Attack:
                    monsters[playerActions[i].target].RecieveAttack(playerActions[i].attack);
                    break;
            }

            playerActions[i].actionType = ActionState.None;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator EnemyPhase()
    {
        currentState = BattleState.EnemyPhase;

        yield return null;

        StartCoroutine(PlayerPhase());
    }

    IEnumerator Victory()
    {
        currentState = BattleState.Victory;

        yield return null;
    }

    IEnumerator Defeat()
    {
        currentState = BattleState.Defeat;

        yield return null;
    }
}
