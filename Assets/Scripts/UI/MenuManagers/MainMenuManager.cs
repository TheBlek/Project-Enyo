using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string _game_scene;

    public void LoadNewGame()
    {
        SceneLoader.LoadScene(_game_scene);
    }
    public void QuitGame()
    {
        GameQuitter.QuitGame();
    }
}
