using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Walker: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private Transform upper_chest;
    [SerializeField] private Transform legs;

    private Animator animator;
    private void Start()
    {
        animator = legs.GetComponent<Animator>();
    }

    public void LookAtMouse(Camera player_camera)
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        Vector3 relativePos = mouse - upper_chest.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        upper_chest.eulerAngles = Vector3.forward * (angle + 90);
    }

    public void Walk(Vector2 input)
    {
        animator.SetBool("Is Walking", input != Vector2.zero);

        float angle = input.x == 0 ? 0 : 90 + Math.Sign(input.x) * Math.Sign(input.y) * 45;

        legs.eulerAngles = Vector3.forward * angle;

        rig.MovePosition((Vector2)transform.position + input * speed * Time.fixedDeltaTime);
    }
}
