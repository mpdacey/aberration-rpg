using UnityEngine;
using TMPro;

public class DamageTextProducer : MonoBehaviour
{
    public GameObject damagePrefab;

    public void ProduceDamageText(int damage, Color textColour) =>
        ProduceText($"{damage}HP", textColour);

    public void ProduceEvasionText(Color textColour) =>
        ProduceText($"Evaded!", textColour);

    private void ProduceText(string text, Color textColour)
    {
        var damageObject = Instantiate(damagePrefab, transform, true);
        if (damageObject.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI textMeshUI))
        {
            textMeshUI.color = textColour;
            textMeshUI.text = text;
        }
        else if (damageObject.transform.GetChild(0).TryGetComponent(out TextMeshPro textMesh))
        {
            textMesh.color = textColour;
            textMesh.text = text;
        }
        damageObject.transform.parent = transform;
        damageObject.transform.localPosition = Vector3.zero;
    }
}
