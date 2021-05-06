using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{

    public void LoadMainMenu()
    {
        SceneLoader.LoadScene("MainMenu");
    }

    public void Quit()
    {
        GameQuitter.QuitGame();
    }

}