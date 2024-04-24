using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DamageVFXController : MonoBehaviour
{
    [SerializeField] private DamageVFXScriptableObject damageVFXStorage;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayDamageVFX(SpellScriptableObject.SpellType spellType)
    {
        if (damageVFXStorage == null) return;

        if (damageVFXStorage.vfxDictionary.ContainsKey(spellType))
            animator.Play(damageVFXStorage.vfxDictionary[spellType]);
        else
            Debug.LogWarning($"Damage VFX of spell type: {System.Enum.GetName(typeof(SpellScriptableObject.SpellType), spellType)}");
    }
}
