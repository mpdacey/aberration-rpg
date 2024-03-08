using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public CombatController combatController;
    public GameObject battleMenuUI;
    public Button[] monsterTargetButtons;
    public GameObject[] partyLineUpUI;

    private void OnEnable()
    {
        combatController.CurrentPartyTurn += SetPartyLayoutPositions;
        combatController.ShowAttackMenu += ShowBattleMenu;
        combatController.ShowTargetIndicator += ShowTargets;
        combatController.ShowSpellsUI += HideAll;
    }

    private void OnDisable()
    {
        combatController.CurrentPartyTurn -= SetPartyLayoutPositions;
        combatController.ShowAttackMenu -= ShowBattleMenu;
        combatController.ShowTargetIndicator -= ShowTargets;
        combatController.ShowSpellsUI -= HideAll;
    }

    private void SetPartyLayoutPositions(int currentMember)
    {
        for (int i = 0; i < partyLineUpUI.Length; i++)
            partyLineUpUI[i].transform.localPosition = new Vector3(partyLineUpUI[i].transform.localPosition.x, i == currentMember ? -65 : -140, 0);
    }

    private void ShowBattleMenu()
    {
        battleMenuUI.SetActive(true);
        battleMenuUI.GetComponentInChildren<Button>().Select();

        HideTargets();
    }

    private void HideBattleMenu() =>
        battleMenuUI.SetActive(false);

    private void ShowTargets(bool[] aliveTargets)
    {
        monsterTargetButtons[0].transform.parent.gameObject.SetActive(true);

        foreach (var monsterTargetButton in monsterTargetButtons)
            monsterTargetButton.interactable = false;

        switch (aliveTargets.Length)
        {
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    monsterTargetButtons[i].interactable = aliveTargets[i];
                    monsterTargetButtons[i].transform.position = Vector3.left * (4.5f - 4.5f * i);
                }
                monsterTargetButtons[0].Select();
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    monsterTargetButtons[i * 2].interactable = aliveTargets[i * 2];
                    monsterTargetButtons[i * 2].transform.position = Vector3.left * (3 - 6 * i);
                }
                monsterTargetButtons[0].Select();
                break;
            case 1:
                monsterTargetButtons[1].interactable = aliveTargets[1];
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
}
