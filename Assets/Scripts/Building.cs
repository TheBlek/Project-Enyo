using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected float cost;
    [SerializeField] protected float maxHP;
    [SerializeField] protected string name;
    [SerializeField] protected Vector2 size;
    

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public Vector2 GetSize()
    {
        return size;
    }
}
