using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI memberName;
    [SerializeField] TextMeshProUGUI memberHP;
    [SerializeField] TextMeshProUGUI memberSP;
    [SerializeField] Image memberActionType;

    public void SetPartyMember(PartyController.PartyMember partyMember)
    {
        var memberNameValue = partyMember.partyMemberBaseStats.combatantName;
        if (memberNameValue.Length > 7)
            memberNameValue = memberNameValue.Substring(0, 7);
        memberName.text = memberNameValue;
        UpdateHealth(partyMember);
        UpdateStamina(partyMember);
    }

    public void SetActionIcon(Sprite actionIcon) =>
        memberActionType.sprite = actionIcon;

    public void UpdateHealth(PartyController.PartyMember partyMember) =>
        memberHP.text = $"{partyMember.currentHP}/{partyMember.partyMemberBaseStats.combatantMaxHealth}";

    public void UpdateStamina(PartyController.PartyMember partyMember) =>
        memberSP.text = $"{partyMember.currentSP}/{partyMember.partyMemberBaseStats.combatantMaxStamina}";
}
