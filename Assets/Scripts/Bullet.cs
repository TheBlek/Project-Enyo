using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Start()
    {
        Destroy(this.gameObject, 3);
    }
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
