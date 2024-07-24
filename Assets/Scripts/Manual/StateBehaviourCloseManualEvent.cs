using System;
using UnityEngine;

namespace Cryptemental.Manual
{
    public class StateBehaviourCloseManualEvent : StateMachineBehaviour
    {
        public static event Action CloseManualEvent;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (CloseManualEvent != null)
                CloseManualEvent.Invoke();
        }
    }
}
