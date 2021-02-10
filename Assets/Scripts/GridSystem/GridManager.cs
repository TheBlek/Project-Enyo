using System.Collections.Generic;
using UnityEngine;
using System;

public interface IGridItem
{

    Vector2Int GridPosition { get; set; }

}

public class GridManager<T> : MonoBehaviour where T : IGridItem
{
    public bool ShowGizmos;

    [SerializeField] protected Vector2 grid_origin;
    [SerializeField] protected Vector2Int grid_size;
    [SerializeField] protected float cell_size = 0.5f;

    protected T[,] grid;
    private void Awake()
    {
        InitGrid();
    }

    public void InitGrid()
    { 
        grid = new T[grid_size.x, grid_size.y];
        grid_origin = -Vector2.one * cell_size * grid_size / 2;

        for (int i = 0; i < grid_size.x; i++)
        {
            for (int j = 0; j < grid_size.y; j++)
            {
                grid[i, j] = (T)Activator.CreateInstance(typeof(T), new object[] { });
                grid[i, j].GridPosition = new Vector2Int(i,j);
            }
        }
    }

    public T[] GetCellsInRect(Vector2 pos, Vector2 size)
    {
        Vector2 left_corner_pos = pos - size * cell_size / 2;
        List<T> cells = new List<T>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                cells.Add(GetCellFromGlobalPosition(left_corner_pos + new Vector2(i, j) * cell_size));
            }
        }
        return cells.ToArray();
    }

    public Vector2Int GetGridPositionFromGlobal(Vector2 global)
    {
        return new Vector2Int((int)((global.x - grid_origin.x) / cell_size), (int)((global.y - grid_origin.y) / cell_size));
    }

    public T GetCellFromGlobalPosition(Vector2 global)
    {
        Vector2Int temp = GetGridPositionFromGlobal(global);
        if (AbnormalGridPosition(temp))
            Debug.Log("Position seems to be abnormal " + temp.x + " " + temp.y);
        return grid[temp.x, temp.y];
    }

    public Vector3 GetGlobalPosition(T cell)
    {
        return grid_origin + (Vector2)cell.GridPosition * cell_size + Vector2.one * cell_size/2;
    }

    public Vector3 SnapGlobalPositionToNearestCell(Vector3 global)
    {
        return GetGlobalPosition(GetCellFromGlobalPosition(global));
    }

    public T[] GetStraightNeighbours(Vector2Int grid_position) // This returns neighbours without corners 
    {
        List<T> neighbours = new List<T>();

        if (grid_position.y + 1 < grid_size.y)
            neighbours.Add(grid[grid_position.x, grid_position.y + 1]);
        if (grid_position.x + 1 < grid_size.x)
            neighbours.Add(grid[grid_position.x + 1, grid_position.y]);
        if (grid_position.y - 1 >= 0)
            neighbours.Add(grid[grid_position.x, grid_position.y - 1]);
        if (grid_position.x - 1 >= 0)
            neighbours.Add(grid[grid_position.x - 1, grid_position.y]);
        return neighbours.ToArray();
    }

    public T[] GetNeighbours(T cell) // This returns neighbours with corners
    {
        List<T> neighbours = new List<T>();
        Vector2Int pos = cell.GridPosition;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!AbnormalGridPosition(pos + new Vector2Int(x, y)))
                    neighbours.Add(grid[pos.x + x, pos.y + y]);
            }
        }
        return neighbours.ToArray();
    }

    public bool AbnormalGridPosition(Vector2Int position)
    {
        return position.x < 0 || position.x >= grid_size.x || position.y < 0 || position.y >= grid_size.y;
    }

    public float GetCellSize() => cell_size;

    public Vector2Int GetMapSize() => grid_size;

    public int GetMapArea() => grid_size.x * grid_size.y;

    private void OnDrawGizmos()
    {
        if (!ShowGizmos)
            return;

        Gizmos.DrawWireCube(grid_origin + (Vector2)grid_size * cell_size / 2, (Vector2)grid_size * cell_size);
        if (grid == null)
            return;

        foreach (T cell in grid)
        {
            Gizmos.DrawWireCube(GetGlobalPosition(cell), Vector3.one * (cell_size - .1f));
        }
    }
}
