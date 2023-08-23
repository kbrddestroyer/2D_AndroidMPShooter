using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance { get => instance; }

    private AsyncOperation operation = null;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator asyncLoadScene(string sceneName, bool allowSceneActivation = true)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = allowSceneActivation;

        while (!operation.isDone) { yield return null; }
    }

    public void LoadScene(string sceneName, bool allowSceneActivation = true)
    {
        StartCoroutine(asyncLoadScene(sceneName, allowSceneActivation));
    }

    public void ConfirmSceneSwitch()
    {
        // Not required if allowSceneActivation was already set to true

        operation.allowSceneActivation = true;
    }
}
