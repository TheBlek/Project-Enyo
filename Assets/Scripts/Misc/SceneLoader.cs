using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    private static string _nextSceneName;

    public static void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        _nextSceneName = name;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene")
        {
            Debug.Log("I'm gonna load " + _nextSceneName);
            List<AsyncOperation> operations = new List<AsyncOperation>
            {
                SceneManager.LoadSceneAsync(_nextSceneName, LoadSceneMode.Additive),
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene())
            };
            FindObjectOfType<ProgressBar>().Proccesses = operations;
        }
        if (scene.name != "Initialization")
            SceneManager.SetActiveScene(scene);
        if (scene.name == _nextSceneName)
        {
            SceneManager.UnloadSceneAsync("LoadingScene");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}