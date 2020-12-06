using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rig;

    public void LookAtMouse(Camera player_camera)
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        Vector3 relativePos = mouse - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * angle;
    }

    public void Walk(Vector2 input)
    {
        rig.MovePosition((Vector2)transform.position + input * speed * Time.deltaTime);
    }
}
