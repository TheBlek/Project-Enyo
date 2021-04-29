using UnityEngine;
using UnityEngine.UI;

public class MetalTracker : MonoBehaviour
{
    private GameManager gameManager;
    private Text counter_field;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        counter_field = GetComponent<Text>();
    }

    private void Update()
    {
        counter_field.text = gameManager.GetMetalCount().ToString();
    }
}
