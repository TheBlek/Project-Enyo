using UnityEngine;
using System.Collections.Generic;

public class Cannon : Building
{
    [SerializeField] private float _fire_radius;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private float _turn_speed;

    private Enemy _target;
    private Transform _barrel_transform;
    private bool _ready_to_shoot;

    private Shooter _shooter;
    private Animator _animator;

    private void Start()
    {
        _barrel_transform = _barrel.transform;
        _shooter = GetComponent<Shooter>();
        _animator = _barrel.GetComponent<Animator>();
        _shooter.OnShoot += PlayShootAnimation;
    }

    public override void SelfUpdate()
    {
        Collider2D[] _near_colliders = Physics2D.OverlapCircleAll(transform.position, _fire_radius);
        _target = GetNearestEnemy(GetAllEnemies(_near_colliders));

        LookAtTarget();

        TryToShoot();
    }

    private void TryToShoot()
    {
        if (!_ready_to_shoot)
            return;

        var data = Physics2D.RaycastAll(transform.position, -_barrel_transform.right, _fire_radius);

        if ( _target == null)
            return;
        if (data.Length >= 2)
            return;

        Debug.Log(data.Length);
        _shooter.Shoot();
    }

    private Enemy[] GetAllEnemies(Collider2D[] colliders)
    {
        List<Enemy> enemies = new List<Enemy>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemies.Add(enemy);
            }
        }
        return enemies.ToArray();
    }

    private Enemy GetNearestEnemy(Enemy[] enemies)
    {
        if (enemies.Length == 0)
            return null;

        Enemy nearest_enemy = enemies[0];
        float nearest_distance = Vector2.Distance(enemies[0].transform.position, transform.position);
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if ( distance < nearest_distance)
            {
                nearest_distance = distance;
                nearest_enemy = enemy;
            }
        }
        return nearest_enemy;
    }

    private void LookAtTarget()
    {
        if (_target == null)
        {
            _ready_to_shoot = false;
            return;
        }

        Vector2 relative_pos = _target.transform.position - transform.position;
        float target_angle = Mathf.Atan2(relative_pos.y, relative_pos.x) * Mathf.Rad2Deg + 180;

        float angle_diff = target_angle - _barrel_transform.eulerAngles.z;

        while (Mathf.Abs(angle_diff) > 180)
        {
            if (target_angle < _barrel_transform.eulerAngles.z)
                target_angle += 360;
            else
                target_angle -= 360;
            angle_diff = target_angle - _barrel_transform.eulerAngles.z;
        }

        if (Mathf.Abs(angle_diff) > _turn_speed * Time.deltaTime)
        {
            _ready_to_shoot = false;
            target_angle = _barrel_transform.eulerAngles.z + _turn_speed * Time.deltaTime * Mathf.Sign(angle_diff);
        }
        else
            _ready_to_shoot = true;

        _barrel_transform.eulerAngles = Vector3.forward * target_angle;
    }

    private void PlayShootAnimation()
    {
        _animator.Play("Shoot");
    }

}
