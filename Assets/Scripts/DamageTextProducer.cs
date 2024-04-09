using UnityEngine;
using TMPro;

public class DamageTextProducer : MonoBehaviour
{
    public GameObject damagePrefab;

    public void ProduceText(string text, Color textColour)
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
        damageObject.transform.SetParent(transform);
        damageObject.transform.localPosition = Vector3.zero;
        damageObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
