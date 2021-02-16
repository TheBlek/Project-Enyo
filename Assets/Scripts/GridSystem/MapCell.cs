using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : IGridItem, IHeapItem<MapCell>
{   
    private Vector2Int grid_position; // should I use this or I can just use autoproperty?

    public int gCost;
    public int hCost;
    public MapCell parent;

    public Building BuildingInCell;
    private bool walkable = true;

    private int heap_index;

    public bool Buildable()
    {
        return BuildingInCell == null;
    }

    public bool IsWalkable() => walkable && BuildingInCell == null;

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

    public Vector2Int GridPosition
    {
        get
        {
            return grid_position;
        }
        set
        {
            grid_position = value;
        }
    }

    public int CompareTo(MapCell cellToCompare)
    {
        int compare = fCost.CompareTo(cellToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(cellToCompare.hCost);
        return -compare;
    }
}
