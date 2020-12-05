using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Camera player_camera;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        Vector3 relativePos = mouse - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * angle;
    }

    private void HandleInput()
    {
        float xInput = Input.GetAxis("Vertical");
        float yInput = Input.GetAxis("Horizontal");
        Vector3 movement = Vector3.up * xInput + Vector3.right * yInput;
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}
