using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : GridManager<MapCell>
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private MapTiles[] keys;
    [SerializeField] private Tile[] values;

    [SerializeField] private float start_freqency;
    [SerializeField] private float lacunarity;
    [Range(0, 1)]
    [SerializeField] private float persistance;
    [SerializeField] private int octaves_count;

    [SerializeField] private Tile[] tile_patterns_grass;
    [SerializeField] private Tile[] tile_patterns_ground;

    private Dictionary<MapTiles, Tile> tiles_by_name;

    
    public override void InitGrid()
    {
        base.InitGrid();
        GenerateMap();
        ConvertArrayToDict();
        SetUpLayout();
        AdjustCornerTiles();
    }
    
    private void ConvertArrayToDict()
    {
        tiles_by_name = new Dictionary<MapTiles, Tile>();

        for (int i = 0; i < keys.Length; i++)
        {
            tiles_by_name[keys[i]] = values[i];
        }
    }

    private void SetUpLayout()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < grid_size.x; x++)
        {
            for (int y = 0; y < grid_size.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x - grid_size.x/2, y - grid_size.y / 2, 0), tiles_by_name[grid[x, y].Tile]);
            }
        }
    }

    private void AdjustCornerTiles()
    {
        NeighbourAdjuster<Tile, MapCell> ground_adjuster = new NeighbourAdjuster<Tile, MapCell>(tile_patterns_ground);
        NeighbourAdjuster<Tile, MapCell> grass_adjuster = new NeighbourAdjuster<Tile, MapCell>(tile_patterns_grass);
        for (int x = 0; x < grid_size.x; x++)
        {
            for (int y = 0; y < grid_size.y; y++)
            {
                MapCell[] neighbours = GetStraightNeighbours(new Vector2Int(x, y));

                Tile new_tile = tiles_by_name[grid[x, y].Tile];
                switch (grid[x, y].Tile)
                {
                    case MapTiles.Grass:
                        new_tile = grass_adjuster.GetResultForSubject(grid[x, y], neighbours);
                        break;
                    case MapTiles.Ground:
                        new_tile = ground_adjuster.GetResultForSubject(grid[x, y], neighbours);
                        break;
                    default:
                        break;
                }
                tilemap.SetTile(new Vector3Int(x - grid_size.x / 2, y - grid_size.y / 2, 0), new_tile);
            }
        }
    }

    private void GenerateMap()
    {
        MapTiles[,] map = MapGenerator.GenerateMap(grid_size, octaves_count, start_freqency, lacunarity, persistance);

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

    public Building GetBuildingInCell(Vector2Int cell)
    {
        if (AbnormalGridPosition(cell))
            return null;
        return grid[cell.x, cell.y].BuildingInCell;
    }
}
