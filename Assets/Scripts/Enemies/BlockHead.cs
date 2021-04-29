using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BlockHead : Enemy
{
    [SerializeField] private float _knock_back = 0.19f;
    [SerializeField] private float _knock_back_cooldown = 0.34f;
    
    private new void Start()
    {
        base.Start();
        GetComponent<MeleeDamager>().OnDamageDeal += ApplyKnockBack;
    }

    private void ApplyKnockBack(Damagable obj)
    {
        KnockBack(_knock_back, transform.position - obj.transform.position);
        Stun(_knock_back_cooldown);
    }
}