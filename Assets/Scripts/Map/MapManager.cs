﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : GridManager<MapCell>
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private MapTiles[] keys;
    [SerializeField] private RuleTile[] values;

    [SerializeField] private float start_freqency;
    [SerializeField] private float lacunarity;
    [Range(0, 1)]
    [SerializeField] private float persistance;
    [SerializeField] private int octaves_count;

    private Dictionary<MapTiles, RuleTile> tiles_by_name;

    
    public override void InitGrid()
    {
        base.InitGrid();
        GenerateMap();
        ConvertArrayToDict();
        SetUpLayout();
    }
    
    private void ConvertArrayToDict()
    {
        tiles_by_name = new Dictionary<MapTiles, RuleTile>();

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
