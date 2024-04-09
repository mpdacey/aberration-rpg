using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    public Color damageColour = new Color(245 / 255f, 102 / 255f, 102 / 255f);
    public Color weakColour = new Color(232 / 255f, 207 / 255f, 84 / 255f);
    public Color evasionColour = new Color(99 / 255f, 224 / 255f, 224 / 255f);
    public Color resistColour = new Color(200 / 255f, 210 / 255f, 210 / 255f);
    public Color nullColour = new Color(207 / 255f, 112 / 255f, 255 / 255f);
    public Color absorbColour = new Color(97 / 255f, 208 / 255f, 37 / 255f);
    public Color repelColour = new Color(68 / 255f, 110 / 255f, 243 / 255f);

    public BattleUIController battleUI;

    private void OnEnable()
    {
        battleUI.DisplayRecievedPlayerDamageEvent += DisplayDamage;
        MonsterController.DisplayRecievedMonsterDamage += DisplayDamage;
        battleUI.DisplayAttackAffinityEvent += DisplayAdditionalText;
        MonsterController.DisplayMonsterAffinityEvent += DisplayAdditionalText;

    }

    private void OnDisable()
    {
        battleUI.DisplayRecievedPlayerDamageEvent -= DisplayDamage;
        MonsterController.DisplayRecievedMonsterDamage -= DisplayDamage;
        battleUI.DisplayAttackAffinityEvent -= DisplayAdditionalText;
        MonsterController.DisplayMonsterAffinityEvent -= DisplayAdditionalText;
    }

    private void DisplayDamage(DamageTextProducer producer, int damageValue) =>
        StartTextCoroutine(producer, $"{damageValue}HP", damageColour, false);

    private void DisplayAdditionalText(DamageTextProducer producer, CombatantScriptableObject.AttributeAffinity affinity)
    {
        switch (affinity)
        {
            case CombatantScriptableObject.AttributeAffinity.Evade:
                StartTextCoroutine(producer, $"Evade", evasionColour, false);
                break;
            case CombatantScriptableObject.AttributeAffinity.Resist:
                StartTextCoroutine(producer, $"Resist", resistColour);
                break;
            case CombatantScriptableObject.AttributeAffinity.Weak:
                StartTextCoroutine(producer, $"Weak", weakColour);
                break;
            case CombatantScriptableObject.AttributeAffinity.Null:
                StartTextCoroutine(producer, $"Null", nullColour, false);
                break;
            case CombatantScriptableObject.AttributeAffinity.Absorb:
                StartTextCoroutine(producer, $"Absorb", absorbColour);
                break;
            case CombatantScriptableObject.AttributeAffinity.Repel:
                StartTextCoroutine(producer, $"Repel", repelColour, false);
                break;
        }
    }

    private void StartTextCoroutine(DamageTextProducer producer, string text, Color textColour, bool delay = true) =>
        StartCoroutine(producer.ProduceText(text, textColour, delay? 0.4f : 0));
}
