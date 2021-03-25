using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private Color hurtColor;
    public float death_offset;

    public Action onHeal;
    public Action onDamage;
    public Action onKill;
    public Action<Damagable> onDestroy;

    public bool is_enemy;

    private float HP;

    private void Start()
    { 
        HP = maxHP;
        onKill += Destroy;
        onDamage += StartWhiteEffect;
        if (hurtColor.a == 0)
            hurtColor = Color.red;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        onDamage();
        if (HP <= 0)
            onKill?.Invoke();
    }

    public void Heal(float heal_amount)
    {
        HP += heal_amount;
        if (HP > maxHP)
            HP = maxHP;

        onHeal?.Invoke();
    }

    private void Destroy()
    {
        Destroy(gameObject, death_offset);
    }

    private void OnDestroy()
    {
        onDestroy?.Invoke(this);
    }

    private void StartWhiteEffect()
    {
        StartCoroutine(DamageColorEffect());
    }

    IEnumerator DamageColorEffect()
    {
        try
        {
            SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in sprites) 
                sprite.color = hurtColor;
            yield return new WaitForSeconds(.05f);
            foreach (SpriteRenderer sprite in sprites)
                sprite.color = Color.white;
        }
        finally { }
    }

    public float GetCurrentHP() => HP;
    public float GetMaxHP() => maxHP;
}
