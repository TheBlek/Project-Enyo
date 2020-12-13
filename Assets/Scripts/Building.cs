using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected int cost;

    public virtual void SelfUpdate(GameManager gameManager)
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

    public int GetCost()
    {
        return cost;
    }
}
