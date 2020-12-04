using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    [SerializeField] protected Building building;
    [SerializeField] protected GameManager gameManager;

    private float grid;
    void Start()
    {
        grid = gameManager.GetGridSize();
        Vector2 size = building.GetSize();
        Vector3 sizeV3 = Vector3.one;
        sizeV3.x *= size.x * grid;
        sizeV3.x -= 0.01f;
        sizeV3.y *= size.y * grid;
        sizeV3.y -= 0.01f;

        transform.localScale = sizeV3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
