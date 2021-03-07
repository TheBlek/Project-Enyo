using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinder
{
    private MapManager mapManager;

    private Action<Vector3[], bool> OnPathProcessingEnd;
    public Pathfinder (MapManager _gridManager, Action<Vector3[], bool> _OnPathProccessingEnd)
    {
        mapManager = _gridManager;
        OnPathProcessingEnd = _OnPathProccessingEnd;
    }

    public IEnumerator FindPath(Vector3 start_pos, Vector3 target_pos) // A* Algorithm
    { 
        MapCell start = mapManager.GetCellFromGlobalPosition(start_pos);
        MapCell target = mapManager.GetCellFromGlobalPosition(target_pos);

        Heap<MapCell> openSet = new Heap<MapCell>(mapManager.GetMapArea());
        HashSet<MapCell> closedSet = new HashSet<MapCell>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            MapCell current = openSet.RemoveFirst();

            closedSet.Add(current);
            
            if (target == current)
            {
                Vector3[] path = RetracePath(start, target);
                OnPathProcessingEnd(path, true);
                yield break;
            }

            foreach (MapCell neighbour in mapManager.GetNeighbours(current))
            {
                if ((!neighbour.IsWalkable() && neighbour != target) || closedSet.Contains(neighbour))
                    continue;

                int newDistanceToNeighbour = current.gCost + GetDistance(current, neighbour);
                if (newDistanceToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newDistanceToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, target);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        yield return null;
        OnPathProcessingEnd(null, false);
    }

    private Vector3[] RetracePath(MapCell start, MapCell end)
    {
        List<MapCell> cellPath = new List<MapCell>();

        MapCell current = end;

        while (current != start)
        {
            cellPath.Add(current);
            current = current.parent;
        }
        cellPath.Add(current);

        cellPath.Reverse();

        return SimplifyPath(cellPath);
    }

    private Vector3[] SimplifyPath(List<MapCell> cellPath)
    {
        Vector3 lastDirection = Vector3.zero;
        List<Vector3> path = new List<Vector3>();

        for (int i = 1; i < cellPath.Count; i++)
        {
            Vector3 direction = (Vector2)cellPath[i].GridPosition - cellPath[i - 1].GridPosition;
            if (direction != lastDirection)
            {
                path.Add(mapManager.GetGlobalPosition(cellPath[i-1]));
                lastDirection = direction;
            }
        }
        path.Add(mapManager.GetGlobalPosition(cellPath[cellPath.Count - 1]));

        return path.ToArray();
    }

    private int GetDistance(MapCell start, MapCell end)
    {
        int distX = Mathf.Abs(start.GridPosition.x - end.GridPosition.x);
        int distY = Mathf.Abs(start.GridPosition.y - end.GridPosition.y);


        if (distX > distY)
            return 14 * distY + 10*(distX - distY);
        return 14 * distX + 10*(distY - distX);
    }
}
