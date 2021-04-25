using UnityEngine;
using System;

public class Walker: MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _legs;
    [SerializeField] private bool _turn_legs;

    private Animator _animator;
    private bool _animate_legs;
    private void Start()
    {
        _animate_legs = _legs.TryGetComponent(out _animator);
    }

    public void WalkForward(float power)
    {
        Walk(transform.right * power);
    }

    public void Walk(Vector2 direction)
    {
        if (direction != Vector2.zero && _animate_legs)
            _animator.Play("Walk");
        else if (_animate_legs)
            _animator.Play("Idle");

        if (_turn_legs)
            TurnLegs(direction);

        CurrentDirection = direction;
        _rigidbody.MovePosition((Vector2)transform.position + direction * _speed * Time.fixedDeltaTime);
    }

    private void TurnLegs(Vector2 direction)
    {
        float angle = direction.x == 0 ? 0 : 90 + Math.Sign(direction.x) * Math.Sign(direction.y) * 45;
        _legs.eulerAngles = Vector3.forward * angle;
    }

    public Vector2 CurrentDirection { get; set; }
}
