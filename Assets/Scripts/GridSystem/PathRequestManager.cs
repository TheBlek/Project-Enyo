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
        pathfinder = new Pathfinder(gameManager.GetGridManager(), OnPathProccessingEnd);
    }

    private struct PathRequest
    {
        public Vector3 start, end;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            start = _start;
            end = _end;
            callback = _callback;
        }
    }

    public void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
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

    public void OnPathProccessingEnd(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        IsProccessingPath = false;
        TryProccessNext();
    }
}
