using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
    public Color damageColour = new Color(0.8f, 0.1f, 0.2f);
    public BattleUIController battleUI;

    private void DisplayRecievedPlayerDamage(int playerIndex, int damageValue) =>
        battleUI.partyLineUpUI[playerIndex].GetComponent<DamageTextProducer>().ProduceDamageText(damageValue * -1, damageColour);
}
