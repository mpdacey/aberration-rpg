using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleUIController : MonoBehaviour
{
    public event Action<DamageTextProducer, int> DisplayRecievedPlayerDamageEvent;
    public event Action<DamageTextProducer> DisplayEvadedAttackEvent;

    public CombatController combatController;
    public GameObject battleMenuUI;
    public Button skillButton;
    public Button[] monsterTargetButtons;
    public PlayerStatsUIController[] partyLineUpUI;
    private int currentSelected;

    private void OnEnable()
    {
        GameController.SetPartyMember += SetPartyValues;
        combatController.CurrentPartyTurn += SetPartyLayoutPositions;
        combatController.ShowAttackMenu += ShowBattleMenu;
        combatController.ShowTargetIndicator += ShowTargets;
        combatController.ShowSpellsUI += HideAll;
        combatController.StatePlayerAttack += HideAll;
        combatController.UpdatePlayerHP += UpdateHealth;
        combatController.UpdatePlayerSP += UpdateStamina;
        combatController.UpdateActionIcon += SetActionIcon;
        combatController.DisplayRecievedPlayerDamage += DisplayRecievedPlayerDamage;
        combatController.DisplayEvadedAttack += DisplayEvadedAttack;
    }

    private void OnDisable()
    {
        GameController.SetPartyMember -= SetPartyValues;
        combatController.CurrentPartyTurn -= SetPartyLayoutPositions;
        combatController.ShowAttackMenu -= ShowBattleMenu;
        combatController.ShowTargetIndicator -= ShowTargets;
        combatController.ShowSpellsUI -= HideAll;
        combatController.StatePlayerAttack -= HideAll;
        combatController.UpdatePlayerHP -= UpdateHealth;
        combatController.UpdatePlayerSP -= UpdateStamina;
        combatController.UpdateActionIcon -= SetActionIcon;
        combatController.DisplayRecievedPlayerDamage -= DisplayRecievedPlayerDamage;
        combatController.DisplayEvadedAttack -= DisplayEvadedAttack;
    }

    private void Update()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (battleMenuUI.activeInHierarchy)
        {
            if (selectedObject == null)
                battleMenuUI.GetComponentsInChildren<Button>()[currentSelected].Select();
            else
                currentSelected = selectedObject.transform.GetSiblingIndex();
        }
        else if (monsterTargetButtons[0].transform.parent.gameObject.activeInHierarchy)
        {
            if (selectedObject == null)
                monsterTargetButtons[currentSelected].Select();
            else
                currentSelected = selectedObject.transform.GetSiblingIndex();
        }
    }

    private void SetPartyValues(PartyController.PartyMember? partyMemberStats, int partyMemberPosition)
    {
        var hasValue = partyMemberStats.HasValue;
        partyLineUpUI[partyMemberPosition].transform.GetChild(0).gameObject.SetActive(hasValue);
        if (hasValue)
            partyLineUpUI[partyMemberPosition].SetPartyMember(partyMemberStats.Value);
    }

    private void SetActionIcon(CombatController.PlayerAction playerAction, int playerIndex)
    {
        Sprite actionTypeSprite = SpellIcons.empty;

        switch (playerAction.actionType)
        {
            case CombatController.ActionState.Attack:
            case CombatController.ActionState.Skill:
                actionTypeSprite = SpellIcons.icons[playerAction.attackAction.attack.attackSpell.spellType];
                break;
            case CombatController.ActionState.Guard:
                actionTypeSprite = SpellIcons.guard;
                break;
        }

        partyLineUpUI[playerIndex].SetActionIcon(actionTypeSprite);
    }

    private void SetPartyLayoutPositions(int currentMember)
    {
        for (int i = 0; i < partyLineUpUI.Length; i++)
        {
            partyLineUpUI[i].transform.localPosition = new Vector3(partyLineUpUI[i].transform.localPosition.x, i == currentMember ? -65 : -105, 0);
        }
    }

    private void ShowBattleMenu(PartyController.PartyMember currentPlayer)
    {
        battleMenuUI.SetActive(true);
        battleMenuUI.GetComponentInChildren<Button>().Select();
        skillButton.interactable = currentPlayer.partyMemberBaseStats.combatantSpells.Count > 0;

        HideTargets();
    }

    private void HideBattleMenu() =>
        battleMenuUI.SetActive(false);

    private void ShowTargets(bool[] aliveTargets, int monsterCount)
    {
        monsterTargetButtons[0].transform.parent.gameObject.SetActive(true);

        foreach (var monsterTargetButton in monsterTargetButtons)
            monsterTargetButton.interactable = false;

        switch (monsterCount)
        {
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    monsterTargetButtons[i].interactable = aliveTargets[i];
                    monsterTargetButtons[i].transform.position = Vector3.left * (1 - 1 * i);
                }
                currentSelected = monsterTargetButtons.First(x => x.interactable == true).transform.GetSiblingIndex();
                monsterTargetButtons[currentSelected].Select();
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    monsterTargetButtons[i * 2].interactable = aliveTargets[i * 2];
                    monsterTargetButtons[i * 2].transform.position = Vector3.left * (0.75f - 1.5f * i);
                }
                currentSelected = monsterTargetButtons.First(x => x.interactable == true).transform.GetSiblingIndex();
                monsterTargetButtons[currentSelected].Select();
                break;
            case 1:
                monsterTargetButtons[1].transform.position = Vector3.zero;
                monsterTargetButtons[1].interactable = aliveTargets[1];
                currentSelected = 1;
                monsterTargetButtons[1].Select();
                break;
        }

        HideBattleMenu();
    }

    private void HideTargets() => monsterTargetButtons[0].transform.parent.gameObject.SetActive(false);

    private void HideAll()
    {
        HideBattleMenu();
        HideTargets();
    }

    private void UpdateHealth(PartyController.PartyMember partyMember, int playerIndex) =>
        partyLineUpUI[playerIndex].UpdateHealth(partyMember);

    private void UpdateStamina(PartyController.PartyMember partyMember, int playerIndex) =>
        partyLineUpUI[playerIndex].UpdateStamina(partyMember);

    private void DisplayRecievedPlayerDamage(int playerIndex, int damageValue)
    {
        if (DisplayRecievedPlayerDamageEvent != null)
            DisplayRecievedPlayerDamageEvent.Invoke(partyLineUpUI[playerIndex].GetComponent<DamageTextProducer>(), damageValue);
    }

    private void DisplayEvadedAttack(int playerIndex)
    {
        if (DisplayEvadedAttackEvent != null)
            DisplayEvadedAttackEvent.Invoke(partyLineUpUI[playerIndex].GetComponent<DamageTextProducer>());
    }
}
