using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "Scriptable Objects/Dice")]
public class DiceScriptableObject : ScriptableObject
{
    public int[] faces;
}
