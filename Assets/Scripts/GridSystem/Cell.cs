using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : IHeapItem<Cell>
{
    private Vector2Int GridPosition;
    
    public int gCost;
    public int hCost;
    public Cell parent;

    public Building BuildingInCell;
    private bool walkable;

    private int heap_index;

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

    public int HeapIndex
    {
        get
        {
            return heap_index;
        }
        set
        {
            heap_index = value;
        }
    }

    public int CompareTo(Cell cellToCompare)
    {
        int compare = fCost.CompareTo(cellToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(cellToCompare.hCost);
        return -compare;
    }
}
