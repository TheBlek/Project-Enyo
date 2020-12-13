using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private void Start()
    {
        Destroy(gameObject, 3);
        transform.GetComponent<Rigidbody>().AddForce(-transform.up * speed);
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.collider.gameObject;
        try
        {
            obj.GetComponent<Damagable>().TakeDamage(damage);
            Destroy(gameObject);
        }
        catch { };
    }

    private void OnTriggerEnter(Collider other)
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
