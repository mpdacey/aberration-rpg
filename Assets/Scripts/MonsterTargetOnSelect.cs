using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterTargetOnSelect : MonoBehaviour, ISelectHandler
{
    public static event Action<int> MonsterTargetSelected;
    public int targetIndex = 0;

    public void OnSelect (BaseEventData eventData)
    {
        if (MonsterTargetSelected != null)
            MonsterTargetSelected.Invoke(targetIndex);
    }
}
