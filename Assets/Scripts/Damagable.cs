using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private Color hurtColor;

    public delegate void OnKill();
    public delegate void OnDamage();
    OnDamage onDamage;
    OnKill onKill;

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
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void StartWhiteEffect()
    {
        StartCoroutine(WhiteColorEffect());
    }

    IEnumerator WhiteColorEffect()
    {
        try
        {
            SpriteRenderer mat = transform.GetComponent<SpriteRenderer>();
            Color col = mat.color;
            mat.color = hurtColor;
            yield return new WaitForSeconds(.05f);
            mat.color = Color.white;
        }
        finally { }
    }
}
