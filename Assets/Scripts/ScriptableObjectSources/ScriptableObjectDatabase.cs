using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "Scriptable Objects/Storage/Scriptable Object Database")]
public class ScriptableObjectDatabase : ScriptableObject
{
    public Dictionary<string, BaseScriptableObject> database;
    [SerializeField] int databaseCount = 0;
    [SerializeField] BaseScriptableObject[] values;

    private void OnValidate()
    {
        if (values.Length == 0) return;

        if (database == null) database = new Dictionary<string, BaseScriptableObject>();

        database.Clear();
        for (int i = 0; i < values.Length; i++)
            if(values[i] != null)
                database.Add(values[i].Id, values[i]);

        databaseCount = database.Count;
    }
}
