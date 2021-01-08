using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected Buildings type;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected int cost;

    public virtual void SetUp(GameManager gameManager)
    {

    }

    public virtual void SelfUpdate(GameManager gameManager)
    {

    }

    public virtual void AdjustTexture()
    {

    }

    public Vector2 GetSize() => size;

    public Buildings GetBuildingType() => type;

    public int GetCost() => cost;
 
}
