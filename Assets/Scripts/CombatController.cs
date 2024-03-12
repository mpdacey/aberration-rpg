using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class CombatController : MonoBehaviour
{
    public static event Action<int> FormationCount;
    public static event Action CombatVictory;
    public event Action<int> CurrentPartyTurn;
    public event Action<PartyController.PartyMember> ShowAttackMenu;
    public event Action ShowAttackMenuUI;
    public event Action<Transform, bool[], int> ShowTargetIndicator;
    public event Action ShowTargetIndicatorUI;
    public event Action<PartyController.PartyMember> ShowSpells;
    public event Action ShowSpellsUI;
    public event Action StatePlayerAttack;
    public event Action<PartyController.PartyMember, int> UpdatePlayerHP;
    public event Action<PartyController.PartyMember, int> UpdatePlayerSP;
    public event Action<PlayerAction, int> UpdateActionIcon;
    public event Action<int, int> DisplayRecievedPlayerDamage;
    public event Action<int> DisplayEvadedAttack;

    public struct PlayerAction
    {
        public ActionState actionType;
        public AttackAction attackAction;
    }

    public struct AttackAction
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
        Confirm,
        Cancel
    }

    public MonsterController[] monsters;
    public FormationScriptableObject formation;
    public ActionState actionState = ActionState.None;
    [SerializeField] private BattleState currentBattleState;
    private Transform playerTransform;
    private PartyController.PartyMember currentMember;
    private int selectedMonster = 0;
    private bool[] monstersAlive = new bool[3];
    public bool[] monstersStunned = new bool[3];
    private PlayerAction[] playerActions = new PlayerAction[4];
    private SpellScriptableObject selectedSpell;
    bool isCancelling = false;

    private void OnEnable()
    {
        FieldMovementController.PlayerTranformChanged += UpdatePlayerTransform;
        MonsterController.MonsterDefeated += MonsterDeathHandling;
        MonsterController.MonsterStunned += StunMonster;
        MonsterEncounterController.ThreatTriggered += SetupCombat;
    }

    private void OnDisable()
    {
        FieldMovementController.PlayerTranformChanged -= UpdatePlayerTransform;
        MonsterController.MonsterDefeated -= MonsterDeathHandling;
        MonsterController.MonsterStunned -= StunMonster;
        MonsterEncounterController.ThreatTriggered -= SetupCombat;
    }

    private void Update()
    {
        isCancelling = Input.GetButtonDown("Cancel");
    }

    private void UpdatePlayerTransform(Transform value) =>
        playerTransform = value;

    public void SetupCombat()
    {
        currentBattleState = BattleState.Initializing;

        FieldMovementController.inBattle = true;

        var monstersParent = monsters[0].transform.parent;
        monstersParent.localRotation = playerTransform.rotation;
        monstersParent.position  = playerTransform.position + playerTransform.rotation * Vector3.forward * 5f;

        foreach (var monster in monsters)
        {
            monster.GetComponent<SpriteRenderer>().enabled = false;
            monster.HideDice();
        }

        switch (formation.monsters.Length)
        {
            case 3:
                for(int i = 0; i < 3; i++)
                {
                    monsters[i].CombatantStats = formation.monsters[i];
                    monsters[i].GetComponent<SpriteRenderer>().enabled = true;
                    monsters[i].transform.localPosition = Vector3.left * (2.25f - 2.25f * i);
                }
                monstersAlive[0] = monstersAlive[1] = monstersAlive[2] = true;
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    monsters[i * 2].CombatantStats = formation.monsters[i];
                    monsters[i * 2].GetComponent<SpriteRenderer>().enabled = true;
                    monsters[i * 2].transform.localPosition = Vector3.left * (1f - 2f * i);
                }
                monstersAlive[0] = monstersAlive[2] = true;
                break;
            case 1:
                monsters[1].CombatantStats = formation.monsters[0];
                monsters[1].GetComponent<SpriteRenderer>().enabled = true;
                monsters[1].transform.localPosition = Vector3.zero;
                monstersAlive[1] = true;
                break;
            default:
                Debug.LogError($"Invalid number of monsters in foundation: {formation.name}");
                return;
        }

        BeginBattle();
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

    private void BeginBattle()
    {
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

        monstersStunned = monstersAlive.Select(x => !x).ToArray();
        yield return GenerateMonsterDice();

        bool actionChosen = false;

        for (int currentPlayerIndex = 0; currentPlayerIndex < 4; currentPlayerIndex += (actionState != ActionState.Cancel) ? 1 : 0)
        {
            if (!PartyController.partyMembers[currentPlayerIndex].HasValue)
                continue;

            currentMember = PartyController.partyMembers[currentPlayerIndex].Value;
            if (CurrentPartyTurn != null)
                CurrentPartyTurn.Invoke(currentPlayerIndex);

            ClearPlayerActions(currentPlayerIndex);

            actionChosen = false;
            while (!actionChosen)
            {
                if (ShowAttackMenu != null)
                    ShowAttackMenu.Invoke(currentMember);
                if (ShowAttackMenuUI != null)
                    ShowAttackMenuUI.Invoke();

                while (actionState == ActionState.None)
                {
                    if (isCancelling) actionState = ActionState.Cancel;
                    yield return new WaitForEndOfFrame();
                }

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
                            if (isCancelling) actionState = ActionState.Cancel;

                            yield return new WaitForEndOfFrame();
                        }

                        if (actionState == ActionState.Cancel) break;

                        AttackObject spellAttackObject = new()
                        {
                            attackerStats = currentMember.partyMemberBaseStats.combatantBaseStats,
                            attackSpell = selectedSpell
                        };

                        if(!selectedSpell.spellMultitarget)
                            // Select Enemy
                            yield return SelectEnemy(currentPlayerIndex, ActionState.Skill, spellAttackObject);
                        else
                        {
                            PlayerAction playerAction = new()
                            {
                                actionType = ActionState.Skill,
                                attackAction = new()
                                {
                                    target = -1,
                                    attack = spellAttackObject
                                }
                            };

                            playerActions[currentPlayerIndex] = playerAction;
                            actionState = ActionState.Confirm;
                        }
                        break;
                    case ActionState.Guard:
                        playerActions[currentPlayerIndex] = new() { actionType = ActionState.Guard };
                        actionState = ActionState.Confirm;
                        break;
                    case ActionState.Cancel:
                        bool partyMemberExists = false;
                        while (!partyMemberExists && currentPlayerIndex > 0)
                        {
                            currentPlayerIndex--;
                            partyMemberExists = PartyController.partyMembers[currentPlayerIndex].HasValue;
                        }
                        break;
                }

                if (UpdateActionIcon != null)
                    UpdateActionIcon.Invoke(playerActions[currentPlayerIndex], currentPlayerIndex);

                actionChosen = actionState == ActionState.Confirm || actionState == ActionState.Cancel;
            }
        }

        CurrentPartyTurn.Invoke(-1);

        // Perform actions
        yield return PlayerActionExecution();

        BattleConditionInspector(EnemyPhase());
    }

    private void ClearPlayerActions(int currentPlayerIndex)
    {
        actionState = ActionState.None;
        playerActions[currentPlayerIndex].actionType = actionState;
        if (UpdateActionIcon != null)
            UpdateActionIcon.Invoke(playerActions[currentPlayerIndex], currentPlayerIndex);
    }

    IEnumerator GenerateMonsterDice()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monstersAlive[i])
            {
                StartCoroutine(monsters[i].GenerateDice());
                yield return new WaitForSeconds(0.75f/2);
            }
        }
        yield return new WaitForSeconds(0.75f);
    }

    private void StunMonster(int enemyIndex)
    {
        monstersStunned[Mathf.Clamp(enemyIndex, 0, monstersStunned.Length - 1)] = true;
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
            ShowTargetIndicator.Invoke(playerTransform, monstersAlive, formation.monsters.Length);
        if (ShowTargetIndicatorUI != null)
            ShowTargetIndicatorUI.Invoke();

        while (actionState == loopState)
        {
            if (isCancelling) actionState = ActionState.Cancel;

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
        if (StatePlayerAttack != null)
            StatePlayerAttack.Invoke();

        for (int i = 0; i < playerActions.Length; i++)
        {
            if (playerActions[i].actionType == ActionState.None)
                continue;

            switch (playerActions[i].actionType)
            {
                case ActionState.Attack:
                case ActionState.Skill:
                    if (!playerActions[i].attackAction.attack.attackSpell.spellMultitarget)
                    {
                        if (monstersAlive[playerActions[i].attackAction.target])
                            monsters[playerActions[i].attackAction.target].RecieveAttack(playerActions[i].attackAction.attack, isStunned: monstersStunned[playerActions[i].attackAction.target]);
                        else
                        {
                            var nextMonsterIndex = GetNextAliveMonster();
                            monsters[nextMonsterIndex].RecieveAttack(playerActions[i].attackAction.attack, isStunned: monstersStunned[nextMonsterIndex]);
                        }
                    }
                    else
                    {
                        for(int j = 0; j < 3; j++)
                        {
                            if (monstersAlive[j])
                                monsters[j].RecieveAttack(playerActions[i].attackAction.attack, isStunned: monstersStunned[j]);
                        }
                    }

                    if(playerActions[i].attackAction.attack.attackSpell.spellCost > 0 && PartyController.partyMembers[i].HasValue)
                    {
                        var temp = PartyController.partyMembers[i].Value;
                        temp.currentSP -= playerActions[i].attackAction.attack.attackSpell.spellCost;
                        PartyController.partyMembers[i] = temp;
                        if (UpdatePlayerSP != null)
                            UpdatePlayerSP.Invoke(PartyController.partyMembers[i].Value, i);
                    }
                    break;
            }

            if(playerActions[i].actionType != ActionState.Guard)
            {
                playerActions[i].actionType = ActionState.None;
                yield return new WaitForSeconds(1f);

                if (UpdateActionIcon != null)
                    UpdateActionIcon.Invoke(playerActions[i], i);
            }

            if (currentBattleState == BattleState.Defeat) break;
            if (monstersAlive.All(x => x == false)) currentBattleState = BattleState.Victory;
            if (currentBattleState == BattleState.Victory) break;
        }
    }

    IEnumerator EnemyPhase()
    {
        currentBattleState = BattleState.EnemyPhase;
        Debug.Log("Enemy Phase");

        int[] playerTargetPriority;
        for (int i = 0; i < 3; i++)
        {
            if (!monstersAlive[i] || monstersStunned[i]) continue;

            // Get Enemy Attacks
            AttackAction enemyAttackObject = new();
            enemyAttackObject.attack = monsters[i].GetAttack();

            if (!enemyAttackObject.attack.attackSpell.spellMultitarget)
            {
                // Determine target
                playerTargetPriority = new int[] { 5, 7, 7, 7 };

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

                for (int target = 0; target < 4; target++)
                {
                    if (random < playerTargetPriority[target])
                    {
                        enemyAttackObject.target = target;
                        break;
                    }
                    random -= playerTargetPriority[target];
                }

                yield return DamagePartyMember(enemyAttackObject.target, enemyAttackObject.attack);
            }
            else
            {
                for(int target = 0; target < 4; target++)
                {
                    if(PartyController.partyMembers[target].HasValue)
                        yield return DamagePartyMember(target, enemyAttackObject.attack);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        BattleConditionInspector(PlayerPhase());
    }

    IEnumerator DamagePartyMember(int target, AttackObject attack)
    {
        AttackObject reflectedAttack = null;
        PartyController.PartyMember targetMember = PartyController.partyMembers[target].Value;
        int oldHealth = targetMember.currentHP;
        var affinityCheck = AttackHandler.CalculateIncomingDamage(attack, PartyController.partyMembers[target].Value.partyMemberBaseStats, ref targetMember.currentHP, out reflectedAttack, isGuarding: playerActions[target].actionType == ActionState.Guard);

        // Display damage
        if (DisplayRecievedPlayerDamage != null && oldHealth - targetMember.currentHP != 0)
            DisplayRecievedPlayerDamage.Invoke(target, targetMember.currentHP - oldHealth);
        if (DisplayEvadedAttack != null && affinityCheck == CombatantScriptableObject.AttributeAffinity.Evade)
            DisplayEvadedAttack.Invoke(target);

        targetMember.currentHP = Mathf.Clamp(targetMember.currentHP, 0, targetMember.partyMemberBaseStats.combatantMaxHealth);
        PartyController.partyMembers[target] = targetMember;

        if (UpdatePlayerHP != null && PartyController.partyMembers[target].HasValue)
            UpdatePlayerHP.Invoke(PartyController.partyMembers[target].Value, target);

        yield return new WaitForSeconds(0.25f);

        // Handle damage.
        if (targetMember.currentHP == 0)
        {
            if (target == 0)
            {
                //Game over
                currentBattleState = BattleState.Defeat;
            }
            else
            {
                PartyController.partyMembers[target] = null;
            }
        }
    }

    private void BattleConditionInspector(IEnumerator nextPhase)
    {
        switch (currentBattleState)
        {
            case BattleState.Victory:
                StartCoroutine(Victory());
                break;
            case BattleState.Defeat:
                StartCoroutine(Defeat());
                break;
            default:
                StartCoroutine(nextPhase);
                break;
        }
    }

    IEnumerator Victory()
    {
        currentBattleState = BattleState.Victory;

        Debug.Log("Victory");

        FieldMovementController.inBattle = false;
        for (int i = 0; i < 4; i++)
            ClearPlayerActions(i);

        if (CombatVictory != null)
            CombatVictory.Invoke();

        yield return null;
    }

    IEnumerator Defeat()
    {
        currentBattleState = BattleState.Defeat;

        Debug.Log("Defeated");

        yield return null;
    }
}
