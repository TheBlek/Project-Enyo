using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    private GridCell[,] grid;
    private Vector2 grid_origin;
    private Vector2Int map_size;
    private float cell_size;

    public void InitGrid(Vector2Int _map_size, float _cell_size)
    {
        map_size = _map_size;
        cell_size = _cell_size;

        grid = new GridCell[map_size.x, map_size.y];
        grid_origin = -Vector2.one * cell_size * map_size / 2;

        for (int i = 0; i < map_size.x; i++)
        {
            for (int j = 0; j < map_size.y; j++)
            {
                grid[i, j] = new GridCell();
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
            grid[cell.x, cell.y].building_in_cell = building;
        }

    }

    public Vector2Int[] GetCellNeighbours(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>
        {
            new Vector2Int(cell.x, cell.y + 1),
            new Vector2Int(cell.x + 1, cell.y),
            new Vector2Int(cell.x, cell.y - 1),
            new Vector2Int(cell.x - 1, cell.y)
        };
        return neighbours.ToArray();
    }

    public bool IsCellBuildable(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= map_size.x || cell.y < 0 || cell.y >= map_size.y)
            return false;
        return grid[cell.x, cell.y].Buildable();
    }
    public Building GetBuildingInCell(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= map_size.x || cell.y < 0 || cell.y >= map_size.y)
            return null;
        return grid[cell.x, cell.y].building_in_cell;
    }
}
