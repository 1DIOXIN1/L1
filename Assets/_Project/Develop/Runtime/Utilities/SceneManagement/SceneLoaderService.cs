using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Develop.Runtime.Utilities.SceneManagement
{
    public class SceneLoaderService
    {
        public IEnumerator LoadAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (operation == null)
                throw new System.InvalidOperationException(
                    $"Failed to load scene '{sceneName}'. Check Build Settings.");

            while (operation.isDone == false)
                yield return null;
        }

        public IEnumerator UnloadAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation == null)
                throw new System.InvalidOperationException(
                    $"Failed to unload scene '{sceneName}'.");

            while (operation.isDone == false)
                yield return null;
        }
    }
}
