using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHP;

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
            Material mat = transform.GetComponent<Renderer>().material;
            Color col = mat.color;
            mat.color = new Color(1, 0, 0, 1f);
            yield return new WaitForSeconds(.1f);
            mat.color = col;
        }
        finally { }
    }
}
