using UnityEngine;
using TMPro;

public class DamageTextProducer : MonoBehaviour
{
    public GameObject damagePrefab;

    public void ProduceDamageText(int damage, Color textColour)
    {
        var damageObject = Instantiate(damagePrefab, transform, true);
        if(damageObject.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI textMeshUI))
        {
            textMeshUI.color = textColour;
            textMeshUI.text = $"{damage}HP";
        }
        else if(damageObject.transform.GetChild(0).TryGetComponent(out TextMeshPro textMesh))
        {
            textMesh.color = textColour;
            textMesh.text = $"{damage}HP";
        }
        damageObject.transform.parent = transform;
        damageObject.transform.localPosition = Vector3.zero;
    }
}
