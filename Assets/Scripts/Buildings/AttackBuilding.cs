using UnityEngine;
using System.Collections.Generic;

public abstract class AttackBuilding : Building
{
    [SerializeField] protected float _max_fire_radius;
    [SerializeField] protected float _min_fire_radius;
    [SerializeField] protected float _turn_speed;
    [SerializeField] protected GameObject _barrel;
    [SerializeField] protected bool _check_for_obstacles;

    protected Enemy _target;
    protected bool _ready_to_shoot;

    protected Shooter _shooter;
    protected Transform _barrel_transform;

    protected void Start()
    {
        _barrel_transform = _barrel.transform;
        _shooter = GetComponent<Shooter>();
    }

    public override void SelfUpdate()
    {
        Collider2D[] _near_colliders = Physics2D.OverlapCircleAll(transform.position, _max_fire_radius);
        _target = GetNearestEnemy(GetAllEnemies(_near_colliders));

        LookAtTarget();

        TryToShoot();
    }

    protected Enemy GetNearestEnemy(Enemy[] enemies)
    {
        if (enemies.Length == 0)
            return null;

        Enemy nearest_enemy = null;
        float nearest_distance = Mathf.Infinity;
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if (distance < nearest_distance && distance > _min_fire_radius)
            {
                nearest_distance = distance;
                nearest_enemy = enemy;
            }
        }
        return nearest_enemy;
    }

    protected Enemy[] GetAllEnemies(Collider2D[] colliders)
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

    protected void TryToShoot()
    {
        if (!_ready_to_shoot || _target == null || (_shooter.IsThereObstacleBeforeTarget(_target.transform.position) && _check_for_obstacles))
            return;

        _shooter.Shoot();
    }

    protected void LookAtTarget()
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

}
