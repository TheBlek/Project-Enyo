using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class SceneLoader : MonoBehaviour
{
    private static string _nextSceneName;

    public static void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync("LoadingScene");
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        _nextSceneName = name;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene")
        {
            SceneManager.SetActiveScene(scene);
            FindObjectOfType<ProgressBar>().Proccesses = new List<AsyncOperation>
            {
                SceneManager.LoadSceneAsync(_nextSceneName)
            };
        }
    }
}