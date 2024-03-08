using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class CombatController : MonoBehaviour
{
    public static event Action<int> FormationCount;
    public event Action<int> CurrentPartyTurn;
    public event Action<PartyController.PartyMember> ShowAttackMenu;
    public event Action ShowAttackMenuUI;
    public event Action<bool[]> ShowTargetIndicator;
    public event Action ShowTargetIndicatorUI;
    public event Action<PartyController.PartyMember> ShowSpells;
    public event Action ShowSpellsUI;
    public event Action HideAllUI;

    private struct PlayerAction
    {
        public ActionState actionType;
        public AttackAction attackAction;
    }

    private struct AttackAction
    {
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
    [SerializeField] private BattleState currentBattleState;
    private PartyController.PartyMember currentMember;
    private int selectedMonster = 0;
    private bool[] monstersAlive;
    private PlayerAction[] playerActions = new PlayerAction[4];
    private SpellScriptableObject selectedSpell;

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
        currentBattleState = BattleState.Initializing;

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
                monstersAlive = new bool[3];
                monstersAlive[0] = monstersAlive[2] = true;
                break;
            case 1:
                monsters[1].CombatantStats = formation.monsters[0];
                monsters[1].GetComponent<SpriteRenderer>().enabled = true;
                monstersAlive = new bool[3];
                monstersAlive[1] = true;
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

    public void SelectSpell(SpellScriptableObject spell) =>
        selectedSpell = spell;

    private void MonsterDeathHandling(int monsterID)
    {
        if(monsterID > monsters.Length)
        {
            Debug.LogError($"Monster ID {monsterID} is out of the monster array scope.");
            return;
        }

        monstersAlive[monsterID] = false;

        if (monstersAlive.All(x => x))
            currentBattleState = BattleState.Victory;
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
        currentBattleState = BattleState.PlayerPhase;
        Debug.Log("Player Phase");

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
                    ShowAttackMenu.Invoke(currentMember);
                if (ShowAttackMenuUI != null)
                    ShowAttackMenuUI.Invoke();
                while (actionState == ActionState.None) yield return new WaitForEndOfFrame();

                switch (actionState)
                {
                    case ActionState.Attack:
                        // Select Enemy
                        yield return SelectEnemy(currentPlayerIndex, ActionState.Attack);
                        break;
                    case ActionState.Skill:
                        // Select Skill
                        selectedSpell = null;

                        if (ShowSpells != null)
                            ShowSpells.Invoke(currentMember);
                        if (ShowSpellsUI != null)
                            ShowSpellsUI.Invoke();

                        while (actionState == ActionState.Skill && selectedSpell == null)
                        {
                            if (Input.GetButtonDown("Cancel"))
                                actionState = ActionState.None;

                            yield return new WaitForEndOfFrame();
                        }

                        if (actionState == ActionState.None) break;

                        AttackObject spellAttackObject = new()
                        {
                            attackerStats = currentMember.partyMemberBaseStats.combatantBaseStats,
                            attackSpell = selectedSpell
                        };

                        // Select Enemy
                        yield return SelectEnemy(currentPlayerIndex, ActionState.Skill, spellAttackObject);
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

    int GetNextAliveMonster()
    {
        int firstAlive = 0;

        foreach (var alive in monstersAlive)
        {
            if (alive) return firstAlive;
            firstAlive++;
        }

        return 2;
    }

    IEnumerator SelectEnemy(int currentPlayerIndex, ActionState loopState, AttackObject attackObject = null)
    {
        if (ShowTargetIndicator != null)
            ShowTargetIndicator.Invoke(monstersAlive);
        if (ShowTargetIndicatorUI != null)
            ShowTargetIndicatorUI.Invoke();

        while (actionState == loopState)
        {
            if (Input.GetButtonDown("Cancel"))
                actionState = ActionState.None;

            yield return new WaitForEndOfFrame();
        }

        if (actionState == ActionState.Confirm)
        {
            PlayerAction playerAction = new()
            {
                actionType = loopState,
                attackAction = new()
                {
                    target = selectedMonster,
                    attack = attackObject ?? AttackHandler.GenerateNormalAttack(currentMember.partyMemberBaseStats)
                }
            };

            playerActions[currentPlayerIndex] = playerAction;
        }
    }

    IEnumerator PlayerActionExecution()
    {
        if (HideAllUI != null)
            HideAllUI.Invoke();

        for (int i = 0; i < playerActions.Length; i++)
        {
            if (currentBattleState == BattleState.Victory || currentBattleState == BattleState.Defeat)
                break;

            if (playerActions[i].actionType == ActionState.None)
                continue;

            switch (playerActions[i].actionType)
            {
                case ActionState.Attack:
                case ActionState.Skill:
                    if(monstersAlive[playerActions[i].attackAction.target])
                        monsters[playerActions[i].attackAction.target].RecieveAttack(playerActions[i].attackAction.attack);
                    else
                        monsters[GetNextAliveMonster()].RecieveAttack(playerActions[i].attackAction.attack);
                    break;
            }

            playerActions[i].actionType = ActionState.None;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator EnemyPhase()
    {
        currentBattleState = BattleState.EnemyPhase;
        Debug.Log("Enemy Phase");

        int[] playerTargetPriority;
        for (int i = 0; i < 3; i++)
        {
            if (!monstersAlive[i]) continue;

            // Get Enemy Attacks
            AttackAction enemyAttackObject = new();
            enemyAttackObject.attack = monsters[i].GetAttack();

            if (enemyAttackObject.attack.attackSpell.spellMultitarget) continue;

            // Determine target
            playerTargetPriority = new int[]{ 5, 7, 7, 7 };

            for (int j = 0; j < 4; j++)
            {
                if (!PartyController.partyMembers[j].HasValue)
                {
                    playerTargetPriority[j] = 0;
                    continue;
                }

                PartyController.PartyMember? partyMember = PartyController.partyMembers[j].Value;

                switch (partyMember?.partyMemberBaseStats.combatantAttributes[enemyAttackObject.attack.attackSpell.spellType])
                {
                    case CombatantScriptableObject.AttributeAffinity.Weak:
                        playerTargetPriority[j] += 2;
                        break;
                    case CombatantScriptableObject.AttributeAffinity.Resist:
                        playerTargetPriority[j] -= 2;
                        break;
                    case CombatantScriptableObject.AttributeAffinity.Null:
                        playerTargetPriority[j] -= 3;
                        break;
                    case CombatantScriptableObject.AttributeAffinity.Absorb:
                    case CombatantScriptableObject.AttributeAffinity.Repel:
                        playerTargetPriority[j] -= 4;
                        break;
                }
            }

            int priorityTotal = playerTargetPriority.Sum();
            int random = Mathf.FloorToInt(UnityEngine.Random.Range(0, priorityTotal));

            for(int target = 0; target < 4; target++)
            {
                if(random < playerTargetPriority[target])
                {
                    enemyAttackObject.target = target;
                    break;
                }
                random -= playerTargetPriority[target];
            }

            AttackObject reflectedAttack = null;
            PartyController.PartyMember targetMember = PartyController.partyMembers[enemyAttackObject.target].Value;
            int oldHealth = targetMember.currentHP;
            AttackHandler.CalculateIncomingDamage(enemyAttackObject.attack, PartyController.partyMembers[enemyAttackObject.target].Value.partyMemberBaseStats, ref targetMember.currentHP, out reflectedAttack);

            // Display damage

            targetMember.currentHP = Mathf.Clamp(targetMember.currentHP, 0, targetMember.partyMemberBaseStats.combatantMaxHealth);
            PartyController.partyMembers[enemyAttackObject.target] = targetMember;

            yield return new WaitForSeconds(1f);

            // Handle damage.
            if (targetMember.currentHP == 0)
            {
                if(enemyAttackObject.target == 0)
                {
                    //Game over
                    currentBattleState = BattleState.Defeat;
                    break;
                }
                else
                {
                    PartyController.partyMembers[enemyAttackObject.target] = null;
                }
            }
        }

        switch (currentBattleState)
        {
            case BattleState.Victory:
                StartCoroutine(Victory());
                break;
            case BattleState.Defeat:
                StartCoroutine(Defeat());
                break;
            default:
                StartCoroutine(PlayerPhase());
                break;
        }
    }

    IEnumerator Victory()
    {
        currentBattleState = BattleState.Victory;

        Debug.Log("Victory");

        yield return null;
    }

    IEnumerator Defeat()
    {
        currentBattleState = BattleState.Defeat;

        Debug.Log("Defeated");

        yield return null;
    }
}
