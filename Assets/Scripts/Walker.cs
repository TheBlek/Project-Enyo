using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private BoxCollider collider;

    private Vector3 previous_pos;

    public void LookAtMouse(Camera player_camera)
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        Vector3 relativePos = mouse - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * angle;
    }

    public void Walk(GameManager gameManager, float xInput, float yInput)
    {
        Vector3 movement = Vector3.up * xInput + Vector3.right * yInput;
        previous_pos = transform.position;
        transform.position += movement * speed * Time.deltaTime;
        if (gameManager.IsIntersectingWithWalls(collider.bounds))
            transform.position = previous_pos;
    }
}
