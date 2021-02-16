using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalTracker : MonoBehaviour
{
    private GameManager gameManager;
    private Text counter_field;

    private void Update()
    {
        gameManager = FindObjectOfType<GameManager>();
        counter_field = GetComponent<Text>();

        if (counter_field.text != gameManager.GetMetalCount().ToString())
        {
            counter_field.text = gameManager.GetMetalCount().ToString();
        }
    }
}
