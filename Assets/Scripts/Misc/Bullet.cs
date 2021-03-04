using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 400;
    [SerializeField] private float damage = 50;
    [SerializeField] private float life_time = 1;

    private Animator animator;
    private bool is_in_animation;

    public bool is_enemy;

    private void Start()
    {
        Destroy(gameObject, life_time);
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * speed);
        animator = transform.GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject collider)
    {
        if (is_in_animation)
            return;

        if (collider.TryGetComponent(out Damagable component) && component.is_enemy != is_enemy)
                component.TakeDamage(damage);

        HandleBlowAnimation();
        Destroy(gameObject, .5f);
    }

    private void HandleBlowAnimation()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        Destroy(GetComponent<Collider2D>());
        
        is_in_animation = true;
        
        animator.Play("Blow");
    }
}
