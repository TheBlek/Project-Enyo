using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Building BuildingInCell { get; set; }
    private Vector2Int GridPosition;

    public Cell(Vector2Int _grid_position)
    {
        GridPosition = _grid_position;
    }

    public bool Buildable()
    {
        return BuildingInCell == null;
    }

    public Vector2Int GetGridPosition() => GridPosition;
}
