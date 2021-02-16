using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : GridManager<MapCell>
{
    
    public bool IsRectBuildable(Vector2 pos, Vector2 size)
    {
        MapCell[] cells = GetCellsInRect(pos, size);

        foreach (MapCell cell in cells)
            if (!cell.Buildable())
                return false;

        return true;
    }

    public void AdjustCellsForBuilding(Building building)
    {
        MapCell[] cells = GetCellsInRect(building.transform.position, building.GetSize());
        foreach (MapCell cell in cells)
            cell.BuildingInCell = building;
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
}
