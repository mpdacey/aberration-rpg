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
        var memberNameValue = partyMember.partyMemberBaseStats.combatantName;
        if (memberNameValue.Length > 7)
            memberNameValue = memberNameValue.Substring(0, 7);
        memberName.text = memberNameValue;
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
