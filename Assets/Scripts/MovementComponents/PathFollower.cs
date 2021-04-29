using UnityEngine;
using System;

[RequireComponent(typeof(Walker))]
public class PathFollower : MonoBehaviour
{
    public Action OnTargetAchieved;

    private Vector2 _path_target = default;
    private Vector2[] _waypoints = null;
    private int _current_waypoint;
    private bool _path_requested = false;

    private PathRequestManager _pathRequestManager;
    private Walker _walker;
    private Looker _looker;

    private void Start()
    {
        _walker = GetComponent<Walker>();
        _looker = GetComponent<Looker>();
        _pathRequestManager = FindObjectOfType<GameManager>().GetPathRequestManager();
    }

    public void FollowPath()
    {
        if (_path_requested)
            return;

        if (_waypoints == null || _current_waypoint >= _waypoints.Length)
        {
            RequestPath();
            return;
        }

        _looker.Target = _waypoints[_current_waypoint];

        if (Vector2.Distance(_waypoints[_current_waypoint], transform.position) <= 0.01f && ++_current_waypoint == _waypoints.Length)
            OnTargetAchived();
        else _walker.Walk((_waypoints[_current_waypoint] - (Vector2)transform.position).normalized);
    }

    private void OnTargetAchived()
    {
        _path_target = default;
        _waypoints = null;
        _current_waypoint = 1;
        OnTargetAchieved?.Invoke();
    }

    private void RequestPath()
    {
        if (_waypoints != null)
            _pathRequestManager.UnreservePath(_waypoints);
        _path_requested = true;
        _pathRequestManager.RequestPath(transform.position, _path_target, OnPathFound);
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
}