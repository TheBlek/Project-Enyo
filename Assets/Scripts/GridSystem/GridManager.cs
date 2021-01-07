using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    private Cell[,] grid;
    private Vector2 grid_origin;
    private Vector2Int map_size;
    private float cell_size;

    public void InitGrid(Vector2Int _map_size, float _cell_size)
    {
        map_size = _map_size;
        cell_size = _cell_size;

        grid = new Cell[map_size.x, map_size.y];
        grid_origin = -Vector2.one * cell_size * map_size / 2;

        for (int i = 0; i < map_size.x; i++)
        {
            for (int j = 0; j < map_size.y; j++)
            {
                grid[i, j] = new Cell(new Vector2Int(i, j));
            }
        }
    }

    public Vector2Int[] GetGridCellsIndexInRect(Vector2 pos, Vector2 size)
    {
        Vector2 left_corner_pos = pos - size * cell_size / 2;
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                cells.Add(GetGridCellIndexFromCoords(left_corner_pos + new Vector2(i, j) * cell_size));
            }
        }
        return cells.ToArray();
    }

    public Vector2Int GetGridCellIndexFromCoords(Vector2 coords)
    {
        return new Vector2Int((int)((coords.x - grid_origin.x) / cell_size), (int)((coords.y - grid_origin.y) / cell_size));
    }

    public void AdjustCellsForBuilding(Building building)
    {
        Vector2Int[] cells = GetGridCellsIndexInRect(building.transform.position, building.GetSize());
        foreach (Vector2Int cell in cells)
        {
            grid[cell.x, cell.y].BuildingInCell = building;
        }

    }

    public Vector2Int[] GetStraightNeighbours(Vector2Int cell) // This returns neghbours without corners. 
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (cell.y + 1 < map_size.y)
            neighbours.Add(new Vector2Int(cell.x, cell.y + 1));
        if (cell.x + 1 < map_size.x)
            neighbours.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y - 1 >= 0)
            neighbours.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.x - 1 >= 0)
            neighbours.Add(new Vector2Int(cell.x - 1, cell.y));
        return neighbours.ToArray();
    }

    public Vector2Int[] GetNeighbours(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!AbnormalPosition(cell + new Vector2Int(x, y)))
                    neighbours.Add(cell + new Vector2Int(x, y));
            }
        }
        return neighbours.ToArray();
    }

    public bool IsCellBuildable(Vector2Int cell)
    {
        if (AbnormalPosition(cell))
            return false;
        return grid[cell.x, cell.y].Buildable();
    }
    public Building GetBuildingInCell(Vector2Int cell)
    {
        if (AbnormalPosition(cell))
            return null;
        return grid[cell.x, cell.y].BuildingInCell;
    }

    public bool AbnormalPosition(Vector2Int cell)
    {
        return cell.x < 0 || cell.x >= map_size.x || cell.y < 0 || cell.y >= map_size.y;
    }
}
