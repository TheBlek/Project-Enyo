using UnityEngine;

[System.Serializable]
public class MapCell : IGridItem, IHeapItem<MapCell>
{   
    public int gCost;
    public int hCost;
    public MapCell parent;

    public bool _walkable_tile = true;

    public void SetTileWalkable(bool walkable_tile)
    {
        _walkable_tile = walkable_tile;
    }

    public bool Buildable()
    {
        return BuildingInCell == null && _walkable_tile;
    }

    public bool IsWalkable() => BuildingInCell == null && _walkable_tile;

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
