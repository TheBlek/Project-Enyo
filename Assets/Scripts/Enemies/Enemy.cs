using UnityEngine;

public enum BehaviourPattern
{
    Local,
    Path
}

[RequireComponent(typeof(Damagable))]
[RequireComponent(typeof(Walker))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MeleeDamager))]
public class Enemy : MonoBehaviour
{
    public bool ShowGizmos;
    [SerializeField] LayerMask _player_mask;
    [SerializeField] protected float _vision_radius;

    protected PathRequestManager _pathRequestManager;
    protected Walker _walker;

    protected Vector2 _path_target = default;
    protected Vector2[] _waypoints = null;
    protected int _current_waypoint;
    protected bool _path_requested = false;

    protected Steer _steer;
    protected BehaviourPattern _behaviour_pattern = BehaviourPattern.Local;

    protected bool _stunned = false;
    protected virtual void Start()
    {
        _pathRequestManager = FindObjectOfType<GameManager>().GetPathRequestManager();
        _walker = GetComponent<Walker>();
        _steer = new Steer(16);
    }

    public virtual void SelfUpdate()
    {
        if (_stunned)
            return;

        if (_behaviour_pattern == BehaviourPattern.Path)
            FollowPath();
        else
            ActLocal();
    }

    public void Update()
    {
        
    }

    protected void ReportAllTargets()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _vision_radius, _player_mask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Damagable damagable) && !damagable.is_enemy)
            { 
                float weight = (_vision_radius - Vector2.Distance(damagable.transform.position, transform.position)) / _vision_radius;
                _steer.AddInterestToVector(damagable.transform.position - transform.position, 2 * weight);
            }
        }
    }

    protected void ReportAllObstructions()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _vision_radius, ~_player_mask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;
            float weight = (_vision_radius - Vector2.Distance(collider.transform.position, transform.position)) / _vision_radius;
            if (weight < 0)
                continue;
            Vector2 danger = collider.transform.position - transform.position;
            _steer.AddInterestToVectorWithDesirableDot(-danger, weight / 2, 0.65f);
            _steer.AddDangerToVectorWithThreshold(danger, weight, 0.65f);
        }
    }

    protected void ActLocal()
    {
        _steer.ResetSteerData();
        ReportAllTargets();
        ReportAllObstructions();
        _steer.NormalizeInterests();
        _steer.NormalizeDangers();

        Vector2 direction = _steer.GetResultDirection();
        _walker.LookAtTarget((Vector2)transform.position + direction);
        _walker.Walk(direction);
    }

    protected virtual void FollowPath()
    {
        if (_path_requested)
            return;

        if (_waypoints == null || _current_waypoint >= _waypoints.Length)
        {
            RequestPath();
            return;
        }

        _walker.LookAtTarget(_waypoints[_current_waypoint]);

        if (Vector2.Distance(_waypoints[_current_waypoint], transform.position) <= 0.01f && ++_current_waypoint == _waypoints.Length)
            OnTargetAchived();
        else _walker.Walk((_waypoints[_current_waypoint] - (Vector2)transform.position).normalized);
    }

    protected void OnTargetAchived()
    {
        _path_target = default;
        _waypoints = null;
        _current_waypoint = 1;
        _behaviour_pattern = BehaviourPattern.Local;
    }

    protected void RequestPath()
    {
        if (_waypoints != null)
            _pathRequestManager.UnreservePath(_waypoints);
        _path_requested = true;
        _pathRequestManager.RequestPath(transform.position, _path_target, OnPathFound);
    }

    private void OnDestroy()
    {
        if (_waypoints != null)
            _pathRequestManager.UnreservePath(_waypoints);
    }

    public void OnPathFound(Vector2[] waypoints, bool success)
    {
        if (success)
        {
            _waypoints = waypoints;
            _pathRequestManager.UnreserveNode(_waypoints[0]);
            _current_waypoint = 1;
            _waypoints[_waypoints.Length - 1] = _path_target;
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

    public Vector2 Target
    {
        get
        {
            return _path_target;
        }
        set
        {
            _path_target = value;
            _waypoints = null;
        }
    }

    public void SetBehaviourPatter(BehaviourPattern pattern)
    {
        _behaviour_pattern = pattern;
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos)
            return;

        _steer.VisualizeInterests(transform.position);
        _steer.VisualizeDangers(transform.position);
        _steer.VisualizePickedDirection(transform.position);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _vision_radius);
        /*Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _waypoints[_current_waypoint]);
        for (int i = _current_waypoint; i < _waypoints.Length - 1; i++)
        {
            Gizmos.DrawCube(_waypoints[i], Vector3.one * 0.1f);
            Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
        }
        Gizmos.DrawCube(_target_pos, Vector3.one * 0.2f);*/
    }
}
