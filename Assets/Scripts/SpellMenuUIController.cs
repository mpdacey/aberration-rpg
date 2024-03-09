using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SpellMenuUIController : MonoBehaviour
{
    struct SpellUIObject
    {
        public Button spellButton;
        public Image spellIcon;
        public TextMeshProUGUI spellName;
        public TextMeshProUGUI spellCost;
    }

    public CombatController combatController;
    private SpellUIObject[] buttonObjects;
    private int currentSelected;

    private void Start()
    {
        InitialiseSpellButtons();
    }

    private void InitialiseSpellButtons()
    {
        buttonObjects = new SpellUIObject[8];

        Transform grid = transform.GetChild(0);
        for (int i = 0; i < grid.childCount; i++)
        {
            Transform currentChild = grid.GetChild(i);

            SpellUIObject currentObject = new();
            currentObject.spellButton = currentChild.GetComponent<Button>();
            currentObject.spellIcon = currentChild.GetComponentInChildren<Image>();
            currentObject.spellName = currentChild.GetComponentsInChildren<TextMeshProUGUI>()[0];
            currentObject.spellCost = currentChild.GetComponentsInChildren<TextMeshProUGUI>()[1];

            buttonObjects[i] = currentObject;
        }
    }

    private void OnEnable()
    {
        combatController.ShowSpells += ShowSpellMenu;
        combatController.ShowAttackMenuUI += HideSpellMenu;
        combatController.ShowTargetIndicatorUI += HideSpellMenu;
        combatController.HideAllUI += HideSpellMenu;
    }

    private void OnDisable()
    {
        combatController.ShowSpells -= ShowSpellMenu;
        combatController.ShowAttackMenuUI -= HideSpellMenu;
        combatController.ShowTargetIndicatorUI -= HideSpellMenu;
        combatController.HideAllUI -= HideSpellMenu;
    }

    private void Update()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (buttonObjects[0].spellButton.transform.parent.gameObject.activeInHierarchy)
        {
            if (selectedObject == null)
                buttonObjects[currentSelected].spellButton.Select();
            else
                currentSelected = selectedObject.transform.GetSiblingIndex();
        }
    }

    private void ShowSpellMenu(PartyController.PartyMember partyMember)
    {
        for(int i = 0; i < 8; i++)
        {
            if(i < partyMember.partyMemberBaseStats.combatantSpells.Count)
                SetSpellButton(partyMember.partyMemberBaseStats.combatantSpells[i], partyMember.currentSP, buttonObjects[i]);
            else
                buttonObjects[i].spellButton.gameObject.SetActive(false);
        }

        buttonObjects[0].spellButton.Select();
        buttonObjects[0].spellButton.transform.parent.gameObject.SetActive(true);
    }

    private void SetSpellButton(SpellScriptableObject spell, int currentSP, SpellUIObject buttonObject)
    {
        buttonObject.spellButton.gameObject.SetActive(true);
        buttonObject.spellName.text = spell.spellName;
        buttonObject.spellCost.text = spell.spellCost.ToString();
        buttonObject.spellButton.interactable = currentSP >= spell.spellCost;
        buttonObject.spellButton.onClick.RemoveAllListeners();
        buttonObject.spellButton.onClick.AddListener(() => combatController.SelectSpell(spell));
    }

    private void HideSpellMenu() =>
        buttonObjects[0].spellButton.transform.parent.gameObject.SetActive(false);
}
