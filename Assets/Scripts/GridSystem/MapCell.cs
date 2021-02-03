﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : GridItem, IHeapItem<MapCell>
{   
    public int gCost;
    public int hCost;
    public MapCell parent;

    public Building BuildingInCell;
    private bool walkable;

    private int heap_index;

    public MapCell (Vector2Int _grid_position) : base(_grid_position)
    {
        walkable = true;
    }

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

    public int CompareTo(MapCell cellToCompare)
    {
        int compare = fCost.CompareTo(cellToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(cellToCompare.hCost);
        return -compare;
    }
}