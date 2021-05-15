using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public static bool GameIsPaused { get; private set; }

    [SerializeField] private GameObject _in_game_ui;
    [SerializeField] private GameObject _pause_menu;

    public void Pause()
    {
        _in_game_ui.SetActive(false);
        _pause_menu.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _in_game_ui.SetActive(true);
        _pause_menu.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        GameQuitter.QuitGame();
    }

    public void LoadMainMenu()
    {
        SceneLoader.LoadScene("MainMenu");
    }
}