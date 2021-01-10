using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private Vector2Int GridPosition;
    
    public int gCost;
    public int hCost;
    public Cell parent;

    public Building BuildingInCell;
    private bool walkable;

    public Cell(Vector2Int _grid_position, bool _walkable)
    {
        GridPosition = _grid_position;
        walkable = _walkable;
    }

    public bool Buildable()
    {
        return BuildingInCell == null;
    }

    public bool IsWalkable() => walkable && BuildingInCell == null;

    public Vector2Int GetGridPosition() => GridPosition;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
