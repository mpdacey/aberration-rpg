using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI memberName;
    [SerializeField] TextMeshProUGUI memberHP;
    [SerializeField] Slider memberHPSlider;
    [SerializeField] TextMeshProUGUI memberSP;
    [SerializeField] Slider memberSPSlider;
    [SerializeField] Image memberActionType;

    public void SetPartyMember(PartyController.PartyMember partyMember)
    {
        memberName.text = partyMember.partyMemberBaseStats.combatantName;
        SetSliders(partyMember);
        UpdateHealth(partyMember);
        UpdateStamina(partyMember);
    }

    public void SetActionIcon(Sprite actionIcon) =>
        memberActionType.sprite = actionIcon;

    public void SetSliders(PartyController.PartyMember partyMember)
    {
        memberHPSlider.maxValue = partyMember.partyMemberBaseStats.combatantMaxHealth;
        memberSPSlider.maxValue = partyMember.partyMemberBaseStats.combatantMaxStamina;
    }

    public void UpdateHealth(PartyController.PartyMember partyMember)
    {
        memberHP.text = $"{partyMember.currentHP}";
        memberHPSlider.value = partyMember.currentHP;
    }

    public void UpdateStamina(PartyController.PartyMember partyMember)
    {
        memberSP.text = $"{partyMember.currentSP}";
        memberSPSlider.value = partyMember.currentSP;
    }
}
