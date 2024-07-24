using UnityEngine;

[CreateAssetMenu(fileName = "New Music", menuName = "Scriptable Objects/Music")]
public class MusicScriptableObject : ScriptableObject
{
    public AudioClip track;
    public bool doesLoop;
    public float loopPointSeconds;
}
