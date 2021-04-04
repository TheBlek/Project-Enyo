﻿using UnityEngine;

[RequireComponent(typeof(Damagable))]
[RequireComponent(typeof(Walker))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MeleeDamager))]
public class Enemy : MonoBehaviour
{
    public bool ShowGizmos;
    [SerializeField] LayerMask _player_mask;
    [SerializeField] protected float _vision_radius;

    protected Vector2 _target_pos = default;
    protected Transform _pursueing_transform = null;
    protected PathRequestManager _pathRequestManager;

    protected Vector2[] _waypoints = null;
    protected int _current_waypoint;
    protected bool _path_requested = false;
    protected Walker _walker;
    protected bool _stunned = false;

    protected Steer _steer;

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

        if (_target_pos != default)
            FollowPath();
        else
            TryFindNewTarget();

    }

    protected void TryFindNewTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _vision_radius, _player_mask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Damagable damagable) && !damagable.is_enemy)
            {
                _target_pos = damagable.transform.position;
                _pursueing_transform = damagable.transform;
            }
        }
    }

    protected void ReportAllObstructions()
    {
        foreach (Vector2 direction in _steer.GetDirections())
        {
            Vector2 raycast_pos = transform.position;
            RaycastHit2D[] hits = Physics2D.RaycastAll(raycast_pos, direction, 0.4f, ~_player_mask);
            if (hits.Length > 1)
                _steer.AddDangerToVector(direction, 1f);
        }
    }

    protected virtual void FollowPath()
    {
        if (_path_requested)
            return;

        if (_pursueing_transform != null && Vector2.Distance(_pursueing_transform.position, transform.position) < _vision_radius)
            _target_pos = _pursueing_transform.position;

        bool no_correct_path = _waypoints == null || _current_waypoint >= _waypoints.Length;

        if ((no_correct_path || _target_pos != _waypoints[_waypoints.Length - 1]) && !_path_requested)
        {
            RequestPath();
            if (no_correct_path)
                return;
        }

        _walker.LookAtTarget(_waypoints[_current_waypoint]);

        _steer.ResetSteerData();
        _steer.AddInterestToVector(_waypoints[_current_waypoint] - (Vector2)transform.position, 1f);
        ReportAllObstructions();
        _steer.NormalizeInterests();

        if (Vector2.Distance(_waypoints[_current_waypoint], transform.position) > 0.01f)
            _walker.Walk(_steer.GetResultDirection());
        else if (++_current_waypoint == _waypoints.Length)
            OnTargetAchived();
        else
            _pathRequestManager.UnreserveNode(_waypoints[_current_waypoint - 1]);
    }

    protected void OnTargetAchived()
    {
        _target_pos = default;
        _waypoints = null;
        _current_waypoint = 1;
    }

    protected void RequestPath()
    {
        if (_waypoints != null)
            _pathRequestManager.UnreservePath(_waypoints);
        _path_requested = true;
        _pathRequestManager.RequestPath(transform.position, _target_pos, OnPathFound);
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

    public Vector2 Target
    {
        get
        {
            return _target_pos;
        }
        set
        {
            _target_pos = value;
            _waypoints = null;
            _pursueing_transform = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos || _waypoints == null)
            return;

        _steer.VisualizeInterests(transform.position);
        _steer.VisualizeDangers(transform.position);
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
