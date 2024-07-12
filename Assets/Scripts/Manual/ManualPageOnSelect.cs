using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cryptemental.Manual
{
    public class ManualPageOnSelect : MonoBehaviour, ISelectHandler
    {
        public static event Action<int> OnManualPageSelect;

        public void OnSelect(BaseEventData eventData)
        {
            if (OnManualPageSelect != null)
                OnManualPageSelect.Invoke(transform.GetSiblingIndex());
        }
    }
}
