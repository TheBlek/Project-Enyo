using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Vertical");   
        float yInput = Input.GetAxis("Horizontal");
        Vector3 movement = Vector3.up * xInput + Vector3.right * yInput;
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
