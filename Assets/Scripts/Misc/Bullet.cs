using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _speed = 400;
    [SerializeField] protected float damage = 50;
    [SerializeField] protected float life_time = 1;

    public Transform _target;

    protected Animator _animator;
    private bool is_in_animation;

    public bool is_enemy;

    private void Start()
    {
        Destroy(gameObject, life_time);
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * _speed);
        _animator = transform.GetComponent<Animator>();
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

        if (collider.TryGetComponent(out Damagable component))
        {
            if (component.is_enemy != is_enemy)
                component.TakeDamage(damage);
            HandleBlowAnimation();
            Destroy(gameObject, .5f);
        }
    }

    protected void HandleBlowAnimation()
    {
        var rb = transform.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.freezeRotation = true;
        transform.GetComponent<BoxCollider2D>().isTrigger = true;
        
        is_in_animation = true;
        
        _animator.Play("Blow");
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
