using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker: MonoBehaviour
{
    [SerializeField] private float speed;

    public void LookAtMouse(Camera player_camera)
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        Vector3 relativePos = mouse - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * angle;
    }

    public void Walk(float xInput, float yInput)
    {
        Vector3 movement = Vector3.up * xInput + Vector3.right * yInput;
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}
