using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 grid_origin;
    [SerializeField] private Vector2Int map_size;
    [SerializeField] private float cell_size = 0.5f;

    private Cell[,] grid;
    private void Awake()
    {
        InitGrid();
    }

    public void InitGrid()
    { 
        grid = new Cell[map_size.x, map_size.y];
        grid_origin = -Vector2.one * cell_size * map_size / 2;

        for (int i = 0; i < map_size.x; i++)
        {
            for (int j = 0; j < map_size.y; j++)
            {
                grid[i, j] = new Cell(new Vector2Int(i, j), true);
            }
        }
    }

    public Cell[] GetCellsInRect(Vector2 pos, Vector2 size)
    {
        Vector2 left_corner_pos = pos - size * cell_size / 2;
        List<Cell> cells = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                cells.Add(GetCellFromGlobalPosition(left_corner_pos + new Vector2(i, j) * cell_size));
            }
        }
        return cells.ToArray();
    }

    public bool IsRectBuildable(Vector2 pos, Vector2 size)
    {
        Cell[] cells = GetCellsInRect(pos, size);

        foreach (Cell cell in cells)
            if (!cell.Buildable())
                return false;

        return true;
    }

    public Vector2Int GetGridPositionFromGlobal(Vector2 global)
    {
        return new Vector2Int((int)((global.x - grid_origin.x) / cell_size), (int)((global.y - grid_origin.y) / cell_size));
    }

    public Cell GetCellFromGlobalPosition(Vector2 global)
    {
        Vector2Int temp = GetGridPositionFromGlobal(global);
        return grid[temp.x, temp.y];
    }

    public Vector3 GetGlobalPosition(Cell cell)
    {
        return grid_origin + (Vector2)cell.GetGridPosition() * cell_size + Vector2.one * cell_size/2;
    }

    public void AdjustCellsForBuilding(Building building)
    {
        Cell[] cells = GetCellsInRect(building.transform.position, building.GetSize());
        foreach (Cell cell in cells)
            cell.BuildingInCell = building;
    }

    public Cell[] GetStraightNeighbours(Vector2Int grid_position) // This returns neighbours without corners 
    {
        List<Cell> neighbours = new List<Cell>();

        if (grid_position.y + 1 < map_size.y)
            neighbours.Add(grid[grid_position.x, grid_position.y + 1]);
        if (grid_position.x + 1 < map_size.x)
            neighbours.Add(grid[grid_position.x + 1, grid_position.y]);
        if (grid_position.y - 1 >= 0)
            neighbours.Add(grid[grid_position.x, grid_position.y - 1]);
        if (grid_position.x - 1 >= 0)
            neighbours.Add(grid[grid_position.x - 1, grid_position.y]);
        return neighbours.ToArray();
    }

    public Cell[] GetNeighbours(Cell cell) // This returns neighbours with corners
    {
        List<Cell> neighbours = new List<Cell>();
        Vector2Int pos = cell.GetGridPosition();

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

    public bool IsCellBuildable(Vector2Int grid_position)
    {
        if (AbnormalGridPosition(grid_position))
            return false;
        return grid[grid_position.x, grid_position.y].Buildable();
    }

    public Building GetBuildingInCell(Vector2Int cell)
    {
        if (AbnormalGridPosition(cell))
            return null;
        return grid[cell.x, cell.y].BuildingInCell;
    }

    public bool AbnormalGridPosition(Vector2Int position)
    {
        return position.x < 0 || position.x >= map_size.x || position.y < 0 || position.y >= map_size.y;
    }

    public float GetCellSize() => cell_size;

    public Vector2Int GetMapSize() => map_size;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(grid_origin + (Vector2)map_size * cell_size / 2, (Vector2)map_size * cell_size);
        if (grid == null)
            return;

        foreach (Cell cell in grid)
        {
            Gizmos.DrawWireCube(GetGlobalPosition(cell), Vector3.one * (cell_size - .1f));
        }
    }
}
