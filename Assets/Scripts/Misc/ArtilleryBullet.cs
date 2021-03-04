using UnityEngine;
using System.Collections;

public class ArtilleryBullet : Bullet
{
    [SerializeField] private float _detonate_time;
    [SerializeField] private float _detonate_radius;
    [SerializeField] private GameObject _explosion_prefab;

    private Rigidbody2D _rb;
    private Explosion _explosion;

    private void Start()
    {
        Destroy(gameObject, life_time);
        _animator = GetComponent<Animator>();
        StartCoroutine(StartDetonateCountdown());
        ApplyForce();
        SetUpExplosion();
    }

    private void SetUpExplosion()
    {
        var gm = Instantiate(_explosion_prefab, transform.position, Quaternion.identity);
        gm.transform.parent = transform;
        gm.TryGetComponent(out _explosion);
        _explosion.SetRadius(2 * _detonate_radius / FindObjectOfType<GameManager>().GetMapManager().GetCellSize());
    }

    private void ApplyForce()
    {
        TryGetComponent(out _rb);
        Vector3 relative_target = (_target.position - transform.position);
        float speed_req = relative_target.magnitude / _detonate_time;
        _rb.velocity = speed_req * relative_target.normalized;
    }

    private IEnumerator StartDetonateCountdown()
    {
        yield return new WaitForSeconds(_detonate_time);
        Blow();
    }

    private void Blow()
    {
        _explosion.Blow();
        _rb.velocity = Vector3.zero;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detonate_radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Damagable component))
            {
                if (component.is_enemy != is_enemy)
                {
                    component.TakeDamage(damage);
                }
            }
        }
        HandleBlowAnimation();
    }

    private new void HandleBlowAnimation()
    {
        _animator.Play("Blow");
    }
}