using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class OfferButton : Button
{
    [SerializeField] UnityEvent m_OnSelect;

    public override void OnSelect(BaseEventData eventData)
    {
        if (m_OnSelect != null)
            m_OnSelect.Invoke();
        base.OnSelect(eventData);
    }
}
