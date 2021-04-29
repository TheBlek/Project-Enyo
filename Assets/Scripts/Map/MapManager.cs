using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

[Serializable]
public class MapManager : GridManager<MapCell>
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private MapGenerator _map_generator;
    [SerializeField] private bool _generate_map_on_start;

    [HideInInspector] public RuleTile[] Tiles;
    [HideInInspector] public bool[] Collidability;

    
    protected override void InitGrid()
    {
        base.InitGrid();
        if (_generate_map_on_start)
            HandleMapGeneration();
        ReadjustWalkability();
    }

    public void HandleMapGeneration()
    {
        if (_grid == null || _grid.GetLength(0) != _grid_size.x || _grid.GetLength(1) != _grid_size.y)
            base.InitGrid();
        GenerateMap();
        SetUpLayout();
    }

    private void SetUpLayout()
    {
        _tilemap.ClearAllTiles();
        for (int x = 0; x < _grid_size.x; x++)
        {
            for (int y = 0; y < _grid_size.y; y++)
            {
                _tilemap.SetTile(new Vector3Int(x - _grid_size.x/2, y - _grid_size.y / 2, 0), Tiles[(int)_grid[x, y].Tile]);

                _grid[x, y].SetTileWalkable(!Collidability[(int)_grid[x, y].Tile]);
            }
        }
    }

    private void ReadjustWalkability()
    {
        for (int x = 0; x < _grid_size.x; x++)
        {
            for (int y = 0; y < _grid_size.y; y++)
            {
                MapTiles tile = (MapTiles)Array.IndexOf(Tiles, _tilemap.GetTile(new Vector3Int(x - _grid_size.x / 2, y - _grid_size.y / 2, 0)));
                _grid[x, y].Tile = tile;
                _grid[x, y].SetTileWalkable(!Collidability[(int)tile]);
            }
        } 
    }

    private void GenerateMap()
    {
        MapTiles[,] map = _map_generator.GenerateMap(_grid_size);

        for (int x = 0; x < _grid_size.x; x++)
        {
            for (int y = 0; y < _grid_size.y; y++)
            {
                _grid[x, y].Tile = map[x, y];
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
        OnGridChange?.Invoke();
    }

    public MapCell[] GetCellsAroundBuilding(Building building)
    {
        List<MapCell> result = new List<MapCell>();

        foreach (MapCell cell in _grid)
            if (cell.BuildingInCell == building)
                foreach (MapCell neighbour in GetNeighboursInCircle(cell.GridPosition, 1))
                    if (neighbour.BuildingInCell != building)
                        result.Add(neighbour);
        return result.ToArray();
    }

    public bool IsCellBuildable(Vector2Int grid_position)
    {
        if (AbnormalGridPosition(grid_position))
            return false;
        return _grid[grid_position.x, grid_position.y].Buildable();
    }

    public Building GetBuildingInCell(Vector2Int grid_position)
    {
        if (AbnormalGridPosition(grid_position))
            return null;
        return _grid[grid_position.x, grid_position.y].BuildingInCell;
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos || _grid == null)
            return;

        Gizmos.color = Color.black * 0.8f;
        for (int x = 0; x < _grid_size.x; x++)
        {
            for (int y = 0; y < _grid_size.y; y++)
            {
                if (_grid[x, y].ReservedForPath)
                    Gizmos.DrawCube(GetGlobalPositionFromGrid(x, y), Vector3.one * 0.4f);
            }
        }
    }
}
