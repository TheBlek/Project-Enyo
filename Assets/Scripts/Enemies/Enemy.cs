using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float ApS;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected LayerMask player_mask;
    
    public virtual void SelfUpdate(GameManager gameManager)
    {

    }

}
