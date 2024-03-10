using UnityEngine;
using TMPro;

public class DamageTextProducer : MonoBehaviour
{
    public GameObject damagePrefab;

    public void ProduceDamageText(int damage, Color textColour)
    {
        var damageObject = Instantiate(damagePrefab, transform, true);
        damageObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{damage}HP";
        damageObject.transform.position = transform.position;
    }
}
