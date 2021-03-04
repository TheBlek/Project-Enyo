using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[Serializable]
public class MapManager : GridManager<MapCell>
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private MapGenerator _map_generator;
    [SerializeField] private bool _generate_map_on_start;

    [HideInInspector] public RuleTile[] tiles_by_name;
    [HideInInspector] public bool[] collidable_tiles;

    
    protected override void InitGrid()
    {
        base.InitGrid();
        if (grid == null || _generate_map_on_start)
            HandleMapGeneration();
        ReadjustWalkability();
    }

    public void HandleMapGeneration()
    {
        if (grid == null)
            InitGrid();
        GenerateMap();
        SetUpLayout();
    }

    private void SetUpLayout()
    {
        _tilemap.ClearAllTiles();
        for (int x = 0; x < grid_size.x; x++)
        {
            for (int y = 0; y < grid_size.y; y++)
            {
                _tilemap.SetTile(new Vector3Int(x - grid_size.x/2, y - grid_size.y / 2, 0), tiles_by_name[(int)grid[x, y].Tile]);

                grid[x, y].SetTileWalkable(!collidable_tiles[(int)grid[x, y].Tile]);
            }
        }
    }

    private void ReadjustWalkability()
    {
        for (int x = 0; x < grid_size.x; x++)
        {
            for (int y = 0; y < grid_size.y; y++)
            {
                grid[x, y].SetTileWalkable(!collidable_tiles[Array.IndexOf(tiles_by_name, _tilemap.GetTile(new Vector3Int(x - grid_size.x / 2, y - grid_size.y / 2, 0)))]);
            }
        } 
    }

    private void GenerateMap()
    {
        MapTiles[,] map = _map_generator.GenerateMap(grid_size);

        for (int x = 0; x < grid_size.x; x++)
        {
            for (int y = 0; y < grid_size.y; y++)
            {
                grid[x, y].Tile = map[x, y];
            }
        }
    }

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

    public Building GetBuildingInCell(Vector2Int grid_position)
    {
        if (AbnormalGridPosition(grid_position))
            return null;
        return grid[grid_position.x, grid_position.y].BuildingInCell;
    }
}
