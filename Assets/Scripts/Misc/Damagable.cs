using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private Color hurtColor;
    public float death_offset;

    public delegate void OnKill();
    public delegate void OnDamage();
    public delegate void OnHeal();

    public OnHeal onHeal;
    public OnDamage onDamage;
    public OnKill onKill;

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
            onKill();
    }

    public void Heal(float heal_amount)
    {
        HP += heal_amount;
        if (HP > maxHP)
            HP = maxHP;

        if (onHeal != null)
            onHeal();
    }

    private void Destroy()
    {
        Destroy(gameObject, death_offset);
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
