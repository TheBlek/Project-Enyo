using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected float cost;
    [SerializeField] protected float maxHP;
    [SerializeField] protected string name;
    [SerializeField] protected Vector2 size;
    public GameManager gameManager;
    
    private float grid;

    void Start()
    {
        grid = gameManager.GetGridSize();
        Vector3 sizeV3 = Vector3.one;
        sizeV3.x *= size.x * grid;
        sizeV3.x -= 0.01f;
        sizeV3.y *= size.y * grid;
        sizeV3.y -= 0.01f;
        transform.localScale = sizeV3;
    }

    public virtual void SelfUpdate()
    {
    }

    public Vector2 GetSize()
    {
        return size;
    }

    public string GetName()
    {
        return name;
    }
}
