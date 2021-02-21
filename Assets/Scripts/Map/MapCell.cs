using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : IGridItem, IHeapItem<MapCell>
{   
    public int gCost;
    public int hCost;
    public MapCell parent;


    public bool Buildable()
    {
        return BuildingInCell == null;
    }

    public bool IsWalkable() => BuildingInCell == null;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Building BuildingInCell { get; set; }

    public MapTiles Tile { get; set; }

    public int HeapIndex { get; set; }

    public Vector2Int GridPosition { get; set; }

    public int CompareTo(MapCell cellToCompare)
    {
        int compare = fCost.CompareTo(cellToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(cellToCompare.hCost);
        return -compare;
    }
}
