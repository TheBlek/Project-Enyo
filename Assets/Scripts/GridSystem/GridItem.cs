using System;
using System.Collections.Generic;
using UnityEngine;
public class GridItem
{
    protected Vector2Int grid_position;

    public GridItem (Vector2Int gridPosition)
    {
        grid_position = gridPosition;
    }

    public Vector2Int GetGridPosition() => grid_position;
}