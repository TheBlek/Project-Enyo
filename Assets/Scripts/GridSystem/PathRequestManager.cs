using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Pathfinder pathfinder;
    private MapManager _mapManager;

    private Queue<PathRequest> requestQueue;
    private PathRequest currentPathRequest;

    private bool IsProccessingPath;
    private Coroutine pathFinding;

    private void Awake()
    {
        requestQueue = new Queue<PathRequest>();
        _mapManager = FindObjectOfType<GameManager>().GetMapManager();
        pathfinder = new Pathfinder(_mapManager, OnPathProccessingEnd);
    }

    private struct PathRequest
    {
        public Vector3 start, end;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
        {
            start = _start;
            end = _end;
            callback = _callback;
        }
    }

    public void RequestPath(Vector2 start, Vector2 end, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        requestQueue.Enqueue(newRequest);
        TryProccessNext();
    }

    private void TryProccessNext()
    {
        if (!IsProccessingPath && requestQueue.Count > 0)
        {
            IsProccessingPath = true;
            currentPathRequest = requestQueue.Dequeue();
            if (pathFinding != null)
                StopCoroutine(pathFinding);
            pathFinding = StartCoroutine(pathfinder.FindPath(currentPathRequest.start, currentPathRequest.end));
        }
    }

    public void OnPathProccessingEnd(Vector2[] path, bool success)
    {
        //if (success)
            //ReservePath(path);

        currentPathRequest.callback(path, success);
        IsProccessingPath = false;
        TryProccessNext();
    }

    private void ReservePath(Vector2[] path)
    {
        foreach (Vector2 node in path)
            _mapManager.GetCellFromGlobalPosition(node).ReservedForPath = true;
    }

    public void UnreserveNode(Vector2 node)
    {
        _mapManager.GetCellFromGlobalPosition(node).ReservedForPath = false;
    }

    public void UnreservePath(Vector2[] path)
    {
        foreach (Vector2 node in path)
            UnreserveNode(node);
    }
}
