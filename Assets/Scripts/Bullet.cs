using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float life_time;

    private Animator animator;

    private void Start()
    {
        Destroy(gameObject, life_time);
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * speed);
        animator = transform.GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        try
        {
            obj.GetComponent<Damagable>().TakeDamage(damage);
            HandleBlowAnimation();
        }
        catch { };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        try
        {
            obj.GetComponent<Damagable>().TakeDamage(damage);
            HandleBlowAnimation();
        }
        catch { };
    }

    private void HandleBlowAnimation()
    {
        var rb = transform.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;

        transform.GetComponent<BoxCollider2D>().isTrigger = true;
        animator.Play("Blow");
        Destroy(gameObject, .5f);
    }
}
