using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    public Color damageColour = new Color(0.8f, 0.1f, 0.2f);
    public Color evasionColour = Color.cyan;
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
