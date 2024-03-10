using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    public Color damageColour = new Color(0.8f, 0.1f, 0.2f);
    public BattleUIController battleUI;

    private void OnEnable()
    {
        BattleUIController.DisplayRecievedPlayerDamage += DisplayDamage;
        MonsterController.DisplayRecievedMonsterDamage += DisplayDamage;
    }

    private void OnDisable()
    {
        BattleUIController.DisplayRecievedPlayerDamage -= DisplayDamage;
        MonsterController.DisplayRecievedMonsterDamage -= DisplayDamage;
    }

    private void DisplayDamage(DamageTextProducer producer, int damageValue) =>
        producer.ProduceDamageText(damageValue, damageColour);
}
