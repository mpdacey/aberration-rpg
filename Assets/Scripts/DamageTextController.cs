using System.Collections;
using System.Collections.Generic;
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
        battleUI.DisplayEvadedAttackEvent += DisplayEvasion;
        MonsterController.DisplayMonsterEvasion += DisplayEvasion;

    }

    private void OnDisable()
    {
        battleUI.DisplayRecievedPlayerDamageEvent -= DisplayDamage;
        MonsterController.DisplayRecievedMonsterDamage -= DisplayDamage;
        battleUI.DisplayEvadedAttackEvent -= DisplayEvasion;
        MonsterController.DisplayMonsterEvasion -= DisplayEvasion;
    }

    private void DisplayDamage(DamageTextProducer producer, int damageValue) =>
        producer.ProduceDamageText(damageValue, damageColour);

    private void DisplayEvasion(DamageTextProducer producer) =>
        producer.ProduceEvasionText(evasionColour);
}
