using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    public event Action<DamageTextProducer, int> DisplayRecievedPlayerDamageEvent;
    public event Action<DamageTextProducer, CombatantScriptableObject.AttributeAffinity> DisplayAttackAffinityEvent;

    public CombatController combatController;
    public GameObject battleMenuUI;
    public Button skillButton;
    public Button[] monsterTargetButtons;
    public Animator riftTransitionAnimator;
    public TextMeshProUGUI floorCounter;
    public MonsterAffinityUIController monsterAffinityUI;

    [Header("Player Battle UI")]
    public PlayerStatsUIController[] partyLineUpUI;
    [SerializeField] private float partyLineUpDefaultY = 231 - 178f;
    [SerializeField] private float partyLineUpActiveY = 231 - 142f;

    private int currentSelected;

    private void OnEnable()
    {
        GameController.SetPartyMember += SetPartyValues;
        GoalRiftController.GoalRiftEntered += PlayTransitionAnimation;
        GoalRiftController.GoalRiftEntered += UpdateFloorCounter;
        SceneController.CombatSceneLoaded += UpdateFloorCounter;
        PartyController.PartyLineUpChanged += SetPartyValues;
        RecruitmentController.UpdatePlayerHP += UpdateHealth;
        combatController.CurrentPartyTurn += SetPartyLayoutPositions;
        combatController.ShowAttackMenu += ShowBattleMenu;
        combatController.ShowTargetIndicator += ShowTargets;
        combatController.ShowSpellsUI += HideAll;
        combatController.StatePlayerAttack += HideAll;
        combatController.UpdatePlayerHP += UpdateHealth;
        combatController.UpdatePlayerSP += UpdateStamina;
        combatController.UpdateActionIcon += SetActionIcon;
        combatController.DisplayRecievedPlayerDamage += DisplayRecievedPlayerDamage;
        combatController.DisplayAttackText += DisplayAttackAffinity;
        combatController.DisplayAttackVFX += DisplayRecievedPlayerDamage;
        combatController.DisplayInspectUI += DisplayInspectUI;
    }

    private void OnDisable()
    {
        GameController.SetPartyMember -= SetPartyValues;
        GoalRiftController.GoalRiftEntered -= PlayTransitionAnimation;
        GoalRiftController.GoalRiftEntered -= UpdateFloorCounter;
        PartyController.PartyLineUpChanged -= SetPartyValues;
        RecruitmentController.UpdatePlayerHP -= UpdateHealth;
        combatController.CurrentPartyTurn -= SetPartyLayoutPositions;
        combatController.ShowAttackMenu -= ShowBattleMenu;
        combatController.ShowTargetIndicator -= ShowTargets;
        combatController.ShowSpellsUI -= HideAll;
        combatController.StatePlayerAttack -= HideAll;
        combatController.UpdatePlayerHP -= UpdateHealth;
        combatController.UpdatePlayerSP -= UpdateStamina;
        combatController.UpdateActionIcon -= SetActionIcon;
        combatController.DisplayRecievedPlayerDamage -= DisplayRecievedPlayerDamage;
        combatController.DisplayAttackText -= DisplayAttackAffinity;
        combatController.DisplayAttackVFX -= DisplayRecievedPlayerDamage;
        combatController.DisplayInspectUI -= DisplayInspectUI;
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
            partyLineUpUI[i].transform.position = new Vector3(partyLineUpUI[i].transform.position.x, i == currentMember ? partyLineUpActiveY: partyLineUpDefaultY, 0);
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

    private void ShowTargets(Transform playerTransform, bool[] aliveTargets, int monsterCount, bool isMultitarget)
    {
        monsterTargetButtons[0].transform.parent.gameObject.SetActive(true);
        monsterTargetButtons[0].transform.parent.position = playerTransform.position;
        monsterTargetButtons[0].transform.parent.rotation = playerTransform.rotation;

        foreach (var monsterTargetButton in monsterTargetButtons)
        {
            monsterTargetButton.interactable = true;
            monsterTargetButton.enabled = true;
        }

        SetTargetPositions(monsterCount);
        if (!isMultitarget) SelectSingleTarget(aliveTargets, monsterCount);
        else SelectAllTargets(aliveTargets);
        HideBattleMenu();
    }

    private void SetTargetPositions(int monsterCount)
    {
        switch (monsterCount)
        {
            case 3:
                for (int i = 0; i < 3; i++)
                    monsterTargetButtons[i].transform.localPosition = Vector3.left * (2.25f - 2.25f * i) / 2;
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                    monsterTargetButtons[i * 2].transform.localPosition = Vector3.left * (1f - 2f * i) / 2;
                break;
            case 1:
                monsterTargetButtons[1].transform.localPosition = Vector3.zero;
                break;
        }
    }

    private void SelectSingleTarget(bool[] aliveTargets, int monsterCount)
    {
        Navigation customNav;

        switch (monsterCount)
        {
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    monsterTargetButtons[i].transform.gameObject.SetActive(aliveTargets[i]);

                    customNav = monsterTargetButtons[i].navigation;
                    customNav.mode = Navigation.Mode.Explicit;
                    for (int j = 0; j < 3; j++)
                        if (aliveTargets[(i + j) % 3])
                            customNav.selectOnLeft = monsterTargetButtons[(i + j) % 3];

                    for (int j = 3; j > 0; j--)
                        if (aliveTargets[(i + j) % 3])
                            customNav.selectOnRight = monsterTargetButtons[(i + j) % 3];

                    monsterTargetButtons[i].navigation = customNav;
                }
                currentSelected = monsterTargetButtons.First(x => x.gameObject.activeInHierarchy == true).transform.GetSiblingIndex();
                monsterTargetButtons[currentSelected].Select();
                break;
            case 2:
                monsterTargetButtons[1].enabled = false;
                for (int i = 0; i < 2; i++)
                {
                    monsterTargetButtons[i * 2].transform.gameObject.SetActive(aliveTargets[i * 2]);

                    customNav = monsterTargetButtons[i * 2].navigation;
                    customNav.mode = Navigation.Mode.Explicit;
                    customNav.selectOnLeft = customNav.selectOnRight = monsterTargetButtons[aliveTargets[(i + 1) % 2 * 2] ? (i + 1) % 2 * 2 : i % 2 * 2];
                    monsterTargetButtons[i * 2].navigation = customNav;
                }
                currentSelected = monsterTargetButtons.First(x => x.gameObject.activeInHierarchy == true).transform.GetSiblingIndex();
                monsterTargetButtons[currentSelected].Select();
                break;
            case 1:
                monsterTargetButtons[1].transform.gameObject.SetActive(true);
                customNav = monsterTargetButtons[1].navigation;
                customNav.mode = Navigation.Mode.None;
                monsterTargetButtons[1].navigation = customNav;
                currentSelected = 1;
                monsterTargetButtons[1].Select();
                break;
        }
    }

    private void SelectAllTargets(bool[] aliveTargets)
    {
        bool setInteractable = false;
        for(int i = 0; i < monsterTargetButtons.Length; i++)
        {
            monsterTargetButtons[i].transform.gameObject.SetActive(aliveTargets[i]);
            monsterTargetButtons[i].interactable = false;

            if (!aliveTargets[i]) continue;

            if (!setInteractable)
            {
                monsterTargetButtons[i].interactable = true;
                monsterTargetButtons[i].Select();
                setInteractable = true;
            }

            Navigation customNav = monsterTargetButtons[i].navigation;
            customNav.mode = Navigation.Mode.None;
            monsterTargetButtons[i].navigation = customNav;
        }
    }

    private void HideTargets()
    {
        monsterTargetButtons[0].transform.parent.gameObject.SetActive(false);
    }

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

    private void DisplayAttackAffinity(CombatantScriptableObject.AttributeAffinity affinity, int playerIndex)
    {
        if (DisplayAttackAffinityEvent != null)
            DisplayAttackAffinityEvent.Invoke(partyLineUpUI[playerIndex].GetComponent<DamageTextProducer>(), affinity);
    }

    private void DisplayRecievedPlayerDamage(int playerIndex, SpellScriptableObject.SpellType spellType)
    {
        partyLineUpUI[playerIndex].GetComponentInChildren<DamageVFXController>().PlayDamageVFX(spellType);
    }

    private void DisplayInspectUI(bool shouldShowUI)
    {
        if(shouldShowUI) monsterAffinityUI.ShowMonsterAffinities();
        else monsterAffinityUI.HideMonsterAffinities();
    }

    private void PlayTransitionAnimation() =>
        riftTransitionAnimator.Play("RiftOverlayUI");

    private void UpdateFloorCounter() =>
        floorCounter.text = $"Realm: {GameController.CurrentLevel + 1}";
}
