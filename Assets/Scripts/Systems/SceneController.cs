using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
    public static event Action CombatSceneLoaded;

    [SerializeField]
    private const int COMBAT_SCENE_INDEX = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadCombatScene());
    }

    IEnumerator LoadCombatScene()
    {
        if (SceneManager.GetSceneByBuildIndex(COMBAT_SCENE_INDEX).isLoaded)
            SceneManager.UnloadSceneAsync(COMBAT_SCENE_INDEX);

        var asyncLoadLevel = SceneManager.LoadSceneAsync(COMBAT_SCENE_INDEX, LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone)
            yield return new WaitForEndOfFrame();

        if (CombatSceneLoaded != null)
            CombatSceneLoaded.Invoke();
    }
}
