using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;

    public delegate void OnKill();
    public OnKill onKill;

    private float HP;

    private void Start()
    {
        HP = maxHP;
        onKill += Destroy;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
            onKill();
    }

    public void Heal(float heal_amount)
    {
        HP += heal_amount;
        if (HP > maxHP)
            HP = maxHP;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
