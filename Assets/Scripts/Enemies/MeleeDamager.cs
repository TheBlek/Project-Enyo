using UnityEngine;
using System;

public class MeleeDamager : MonoBehaviour
{
    [SerializeField] private float _melee_damage;
    [SerializeField] private float _attack_speed;
    [SerializeField] private bool _is_enemy;

    public Action<Damagable> OnDamageDeal;

    private bool _ready_to_deal_damage = true;
    private Animator _animator;
    private bool _animate;

    private void Start()
    {
        _animate = TryGetComponent(out _animator);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    private void HandleCollision(Collider2D collider)
    {
        if (collider.TryGetComponent(out Damagable damagable) && damagable.is_enemy != _is_enemy && _ready_to_deal_damage)
            DealMeleeDamage(damagable);
    }

    private void DealMeleeDamage(Damagable damagable)
    {
        _ready_to_deal_damage = false;
        Invoke(nameof(ResetReadyness), 1 / _attack_speed);

        damagable.TakeDamage(_melee_damage);

        if (_animate)
            _animator.Play("Attack");

        OnDamageDeal?.Invoke(damagable);
    }

    private void ResetReadyness()
    {
        _ready_to_deal_damage = true;
    }
}
