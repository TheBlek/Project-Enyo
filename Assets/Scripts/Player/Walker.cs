using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Walker: MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _legs;
    [SerializeField] private bool _turn_legs;
    [SerializeField] private float _sight_offset;

    private Animator _animator;
    private bool _animate_legs;
    private void Start()
    {
        _animate_legs = _legs.TryGetComponent(out _animator);
    }

    public void LookAtMouse(Camera player_camera)
    {
        Vector3 mouse = player_camera.ScreenToWorldPoint(Input.mousePosition);
        LookAtTarget(mouse);
    }

    public void LookAtTarget(Vector2 target)
    {
        Vector3 relativePos = target - (Vector2)_head.position;

        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        _head.eulerAngles = Vector3.forward * (angle + _sight_offset);
    }

    public void Walk(Vector2 direction)
    {
        if (direction != Vector2.zero && _animate_legs)
            _animator.Play("Walk");
        else if (_animate_legs)
            _animator.Play("Idle");

        if (_turn_legs)
            TurnLegs(direction);

        _rigidbody.MovePosition((Vector2)transform.position + direction * _speed * Time.fixedDeltaTime);
    }

    private void TurnLegs(Vector2 direction)
    {
        float angle = direction.x == 0 ? 0 : 90 + Math.Sign(direction.x) * Math.Sign(direction.y) * 45;
        _legs.eulerAngles = Vector3.forward * angle;
    }
}
