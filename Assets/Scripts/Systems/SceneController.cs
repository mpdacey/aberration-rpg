using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
    public static event Action CombatSceneLoaded;
    public static event Action TitleSceneLoaded;

    private const int COMBAT_SCENE_INDEX = 1;
    private const int TITLE_SCENE_INDEX = 2;

    public IEnumerator LoadTitleScene()
    {
        UnloadScenes();

        var asyncLoadLevel = SceneManager.LoadSceneAsync(TITLE_SCENE_INDEX, LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone)
            yield return new WaitForEndOfFrame();

        if (TitleSceneLoaded != null)
            TitleSceneLoaded.Invoke();
    }

    public IEnumerator LoadCombatScene()
    {
        UnloadScenes();

        var asyncLoadLevel = SceneManager.LoadSceneAsync(COMBAT_SCENE_INDEX, LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone)
            yield return new WaitForEndOfFrame();

        if (CombatSceneLoaded != null)
            CombatSceneLoaded.Invoke();
    }

    private void UnloadScenes()
    {
        if (SceneManager.GetSceneByBuildIndex(COMBAT_SCENE_INDEX).isLoaded)
            SceneManager.UnloadSceneAsync(COMBAT_SCENE_INDEX);

        if (SceneManager.GetSceneByBuildIndex(TITLE_SCENE_INDEX).isLoaded)
            SceneManager.UnloadSceneAsync(TITLE_SCENE_INDEX);
    }
}
