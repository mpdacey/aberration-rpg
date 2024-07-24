using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace Cryptemental.SceneController
{
    public static class SceneController
    {
        public static event Action CombatSceneLoaded;
        public static event Action TitleSceneLoaded;
        public static event Action ManualSceneLoaded;
        public static event Action ManualSceneUnloaded;

        private const int COMBAT_SCENE_INDEX = 1;
        private const int TITLE_SCENE_INDEX = 2;
        private const int MANUAL_SCENE_INDEX = 3;

        public static IEnumerator LoadTitleScene()
        {
            UnloadScenes();

            yield return LoadScene(TITLE_SCENE_INDEX);

            if (TitleSceneLoaded != null)
                TitleSceneLoaded.Invoke();
        }

        public static IEnumerator LoadCombatScene()
        {
            UnloadScenes();

            yield return LoadScene(COMBAT_SCENE_INDEX);

            if (CombatSceneLoaded != null)
                CombatSceneLoaded.Invoke();
        }

        public static IEnumerator LoadManualScene()
        {
            if (ManualSceneLoaded != null)
                ManualSceneLoaded.Invoke();

            yield return LoadScene(MANUAL_SCENE_INDEX);
        }

        public static IEnumerator UnloadManualScene()
        {
            if (SceneManager.GetSceneByBuildIndex(MANUAL_SCENE_INDEX).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(MANUAL_SCENE_INDEX);

                if (ManualSceneUnloaded != null)
                    ManualSceneUnloaded.Invoke();
            }
        }

        private static IEnumerator LoadScene(int sceneIndex)
        {
            var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone)
                yield return new WaitForEndOfFrame();
        }

        private static void UnloadScenes()
        {
            UnloadScene(COMBAT_SCENE_INDEX);
            UnloadScene(TITLE_SCENE_INDEX);
            UnloadScene(MANUAL_SCENE_INDEX);
        }

        private static void UnloadScene(int sceneIndex)
        {
            if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
                SceneManager.UnloadSceneAsync(sceneIndex);
        }
    }
}
