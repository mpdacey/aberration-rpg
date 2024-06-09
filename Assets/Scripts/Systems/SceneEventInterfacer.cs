using System;
using UnityEngine;

[RequireComponent(typeof(SceneController))]
public class SceneEventInterfacer : MonoBehaviour
{
    SceneController sceneController;

    private void Start()
    {
        sceneController = GetComponent<SceneController>();
    }

    private void OnEnable()
    {
        TitleManager.PlayButtonPressed += CallCombatScene;
    }

    private void OnDisable()
    {
        TitleManager.PlayButtonPressed -= CallCombatScene;
    }

    private void CallCombatScene()
    {
        StartCoroutine(sceneController.LoadCombatScene());
    }

    private void CallTitleScene()
    {
        StartCoroutine(sceneController.LoadTitleScene());
    }
}
