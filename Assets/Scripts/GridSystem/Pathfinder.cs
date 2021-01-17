using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Pathfinder
{
    private GridManager gridManager;

    private Action<Vector3[], bool> OnPathProcessingEnd;
    public Pathfinder (GridManager _gridManager, Action<Vector3[], bool> _OnPathProccessingEnd)
    {
        gridManager = _gridManager;
        OnPathProcessingEnd = _OnPathProccessingEnd;
    }

    public IEnumerator FindPath(Vector3 start_pos, Vector3 target_pos) // A* Algorithm
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Cell start = gridManager.GetCellFromGlobalPosition(start_pos);
        Cell target = gridManager.GetCellFromGlobalPosition(target_pos);

        Heap<Cell> openSet = new Heap<Cell>(gridManager.GetMapArea());
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Cell current = openSet.RemoveFirst();

            closedSet.Add(current);
            
            if (target == current)
            {
                Vector3[] path = RetracePath(start, target);
                OnPathProcessingEnd(path, true);
                sw.Stop();
                //UnityEngine.Debug.Log("Path found: " + sw.ElapsedMilliseconds + "ms");
                yield break;
            }

            foreach (Cell neighbour in gridManager.GetNeighbours(current))
            {
                if ((!neighbour.IsWalkable() && neighbour != target) || closedSet.Contains(neighbour))
                {
                    continue;
                }

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

    private Vector3[] RetracePath(Cell start, Cell end)
    {
        List<Cell> cellPath = new List<Cell>();

        Cell current = end;
        cellPath.Add(current);
        while (current != start)
        {
            cellPath.Add(current);
            current = current.parent;
        }
        cellPath.Add(current);
        cellPath.Reverse();
        
        return SimplifyPath(cellPath);
    }

    private Vector3[] SimplifyPath(List<Cell> cellPath)
    {
        Vector3 lastDirection = Vector3.zero;
        List<Vector3> path = new List<Vector3>();
        
        for (int i = 1; i < cellPath.Count; i++)
        {
            Vector3 direction = (Vector2)cellPath[i].GetGridPosition() - cellPath[i - 1].GetGridPosition();
            if (direction != lastDirection)
            {
                path.Add(gridManager.GetGlobalPosition(cellPath[i - 1]));
                lastDirection = direction;
            }
        }
        return path.ToArray();
    }

    private int GetDistance(Cell start, Cell end)
    {
        int distX = Mathf.Abs(start.GetGridPosition().x - end.GetGridPosition().x);
        int distY = Mathf.Abs(start.GetGridPosition().y - end.GetGridPosition().y);


        if (distX > distY)
            return 14 * distY + 10*(distX - distY);
        return 14 * distX + 10*(distY - distX);
    }
}
