using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private GridManager gridManager;
    private Vector3[] waypoints;

    [SerializeField] private Transform start;
    [SerializeField] private Transform target;

    private void Start()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Update()
    {
        waypoints = FindPath(start.position, target.position);
    }

    public Vector3[] FindPath(Vector3 start_pos, Vector3 target_pos) // A* Algorithm
    {
        Cell start = gridManager.GetCellFromGlobalPosition(start_pos);
        Cell target = gridManager.GetCellFromGlobalPosition(target_pos);

        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Cell current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost || (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);
            
            if (target == current)
            {
                return RetracePath(start, target);
            }

            foreach (Cell neighbour in gridManager.GetNeighbours(current))
            {
                if (!neighbour.IsWalkable() || closedSet.Contains(neighbour))
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
        return null;
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

    private void OnDrawGizmos()
    {
        if (waypoints == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(waypoints[0], Vector3.one * 0.1f);
        for (int i = 1; i < waypoints.Length; i++)
        {
            Gizmos.DrawCube(waypoints[i], Vector3.one * 0.1f);
            Gizmos.DrawLine(waypoints[i], waypoints[i - 1]);
        }
    }
}
