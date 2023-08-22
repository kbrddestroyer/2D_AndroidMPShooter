using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation operation = null;

    private IEnumerator asyncLoadScene(string sceneName, bool allowSceneActivation = true)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = allowSceneActivation;

        while (!operation.isDone) { yield return null; }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(asyncLoadScene(sceneName));
    }

    public void ConfirmSceneSwitch()
    {
        // Not required if allowSceneActivation was already set to true

        operation.allowSceneActivation = true;
    }
}
