using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float live_time;

    private void Start()
    {
        Destroy(gameObject, live_time);
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * speed);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        try
        {
            obj.GetComponent<Damagable>().TakeDamage(damage);
            Destroy(gameObject);
        }
        catch { };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        try
        {
            obj.GetComponent<Damagable>().TakeDamage(damage);
            Destroy(gameObject);
        }
        catch { };
    }
}
