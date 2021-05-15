using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadSceneAsync("Initialization", LoadSceneMode.Additive);
        SceneLoader.LoadScene("MainMenu");
    }
}