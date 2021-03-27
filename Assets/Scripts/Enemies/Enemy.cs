using UnityEngine;

[RequireComponent(typeof(Damagable))]
[RequireComponent(typeof(Walker))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MeleeDamager))]
public class Enemy : MonoBehaviour
{
    public bool ShowGizmos;

    [SerializeField] protected float _vision_radius;

    protected Vector2 _target_pos = default;
    protected Transform _pursueing_transform = null;
    protected PathRequestManager _pathRequestManager;

    protected Vector2[] _waypoints = null;
    protected int _current_waypoint;
    protected bool _path_requested = false;
    protected Walker _walker;
    protected bool _stunned = false;

    protected virtual void Start()
    {
        _pathRequestManager = FindObjectOfType<GameManager>().GetPathRequestManager();
        _walker = GetComponent<Walker>();
    }

    public virtual void SelfUpdate()
    {
        if (_stunned)
            return;

        if (_target_pos != default)
            MoveToTarget();
        else
            TryFindNewTarget();

    }

    protected void TryFindNewTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _vision_radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Damagable damagable) && !damagable.is_enemy)
            {
                _target_pos = damagable.transform.position;
                _pursueing_transform = damagable.transform;
            }
        }
    }

    protected virtual void MoveToTarget()
    {
        if (_path_requested)
            return;

        if (_pursueing_transform != null && Vector3.Distance(_pursueing_transform.position, transform.position) < _vision_radius)
        {
            _target_pos = _pursueing_transform.position;
        }

        if ((_waypoints == null || _target_pos != _waypoints[_waypoints.Length - 1]) && !_path_requested)
        {
            RequestPath();
            if (_waypoints == null)
                return;
        }

        _walker.LookAtTarget(_waypoints[_current_waypoint]);

        if (Vector2.Distance(_waypoints[_current_waypoint], transform.position) > 0.01f)
            _walker.Walk((_waypoints[_current_waypoint] - (Vector2)transform.position).normalized);
        else if (++_current_waypoint == _waypoints.Length)
            OnTargetAchived();
    }

    protected void OnTargetAchived()
    {
        _target_pos = default;
        _waypoints = null;
        _current_waypoint = 1;
    }

    protected void RequestPath()
    {
        _path_requested = true;
        _pathRequestManager.RequestPath(transform.position, _target_pos, OnPathFound);
    }

    public void OnPathFound(Vector2[] waypoints, bool success)
    {
        if (success)
        {
            _waypoints = waypoints;
            _current_waypoint = 1;
            _waypoints[_waypoints.Length - 1] = _target_pos;
        }
        _path_requested = false;
    }

    protected void KnockBack(float knock_back_distance, Vector2 direction)
    {
        transform.position = (Vector2)transform.position + direction.normalized * knock_back_distance;
    }

    protected void Stun(float duration)
    {
        _stunned = true;
        Invoke(nameof(ResetStun), duration);
    }

    private void ResetStun()
    {
        _stunned = false;
    }

    public void SetTarget(Vector2 target_pos)
    {
        _target_pos = target_pos;
        _waypoints = null; // useless with new target
        _pursueing_transform = null;
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos || _waypoints == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _waypoints[_current_waypoint]);
        for (int i = _current_waypoint; i < _waypoints.Length - 1; i++)
        {
            Gizmos.DrawCube(_waypoints[i], Vector3.one * 0.1f);
            Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
        }
        Gizmos.DrawCube(_target_pos, Vector3.one * 0.2f);
    }
}
