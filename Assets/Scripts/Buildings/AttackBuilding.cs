using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Looker))]
public abstract class AttackBuilding : Building
{
    [SerializeField] protected float _max_fire_radius;
    [SerializeField] protected float _min_fire_radius;
    [SerializeField] protected bool _check_for_obstacles;

    protected Shooter _shooter;
    protected Looker _looker;

    protected new void Start()
    {
        base.Start();
        _shooter = GetComponent<Shooter>();
        _looker = GetComponent<Looker>();
    }

    public override void SelfUpdate()
    {
        TryToShoot();

        Collider2D[] _near_colliders = Physics2D.OverlapCircleAll(transform.position, _max_fire_radius);
        Enemy target = GetNearestEnemy(GetAllEnemies(_near_colliders));

        if (target != null) _looker.Target = target.transform.position;
        else _looker.Target = transform.position;
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
            if (collider.TryGetComponent<Enemy>(out var enemy))
                enemies.Add(enemy);

        return enemies.ToArray();
    }

    protected void TryToShoot()
    {
        if (!_looker.IsPointedOnTarget || _looker.Target == (Vector2)transform.position || (_shooter.IsThereObstacleBeforeTarget(_looker.Target) && _check_for_obstacles))
            return;

        _shooter.Shoot(_looker.Target);
    }
}
