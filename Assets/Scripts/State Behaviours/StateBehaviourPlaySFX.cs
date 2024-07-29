using UnityEngine;
using Cryptemental.Audio;

public class StateBehaviourPlaySFX : StateMachineBehaviour
{
    public AudioClip sfx;
    public bool randomPitch;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sfx != null)
            AudioManager.PlayAudioClip(sfx, randomPitch);
    }
}

