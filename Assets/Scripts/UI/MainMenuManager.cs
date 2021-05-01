using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] string _game_scene;
    [SerializeField] GameObject _buttons;
    [SerializeField] Image _progress_bar;

    private List<AsyncOperation> _loading_scenes;

    public void LoadNewGame()
    {
        HideButtons();
        OpenProgressBar();
        _loading_scenes = new List<AsyncOperation>();
        _loading_scenes.Add(SceneManager.LoadSceneAsync(_game_scene));
        StartCoroutine(nameof(UpdateProgressBar));
    }

    public IEnumerator UpdateProgressBar()
    {
        float value = 0f;
        _progress_bar.fillAmount = value;
        while (value < 1f)
        {
            value = 0f;
            foreach (AsyncOperation scene in _loading_scenes)
                value += scene.progress;
            value /= _loading_scenes.Count;
            _progress_bar.fillAmount = value;
            yield return null;
        }
        yield break;
    }

    public void HideButtons()
    {
        _buttons.SetActive(false);
    }

    public void OpenProgressBar()
    {
        _progress_bar.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
