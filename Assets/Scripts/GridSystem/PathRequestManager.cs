using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Pathfinder pathfinder;
    private GameManager gameManager;

    private Queue<PathRequest> requestQueue;
    private PathRequest currentPathRequest;

    private bool IsProccessingPath;
    private Coroutine pathFinding;

    private void Awake()
    {
        requestQueue = new Queue<PathRequest>();
        gameManager = FindObjectOfType<GameManager>();
        pathfinder = new Pathfinder(gameManager.GetMapManager(), OnPathProccessingEnd);
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
        currentPathRequest.callback(path, success);
        IsProccessingPath = false;
        TryProccessNext();
    }
}
