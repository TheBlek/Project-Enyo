using System.Collections.Generic;
using UnityEngine;
using System;

public enum MapTypes
{
    Mineral,
    Influence,
    PlayerBuildings
}

public class State
{

    private MapManager _mapManager;
    private Vector2Int _map_size;
    private float _max_distance;

    private Dictionary<MapTypes, float[,]> _maps;

    public Action OnStateChange;

    public State(MapManager mapManager)
    {
        _mapManager = mapManager;
        _mapManager.OnGridChange += UpdateMaps;
        _map_size = _mapManager.GetMapSize();
        _maps = new Dictionary<MapTypes, float[,]>();
        UpdateMaps();
    }

    public void UpdateMaps()
    {
        foreach (MapTypes type in (MapTypes[]) Enum.GetValues(typeof(MapTypes)))
        {
            switch (type)
            {
                case MapTypes.Mineral: _maps[type] = GetMineralMap(); break;
                case MapTypes.Influence: _maps[type] = GetInfluenceMap(); break;
                case MapTypes.PlayerBuildings: _maps[type] = GetPlayerBuildings(); break;
                default: break;
            }
        }
        OnStateChange?.Invoke();
    }

    private float[,] GetPlayerBuildings()
    {
        float[,] map = new float[_map_size.x, _map_size.y];

        MapCell[] buildings = GetAllTargetCells((x) => x.BuildingInCell != null);

        foreach (MapCell building in buildings)
        {
            map[building.GridPosition.x, building.GridPosition.y] = 1f;
        }
        return map;
    }

    private float[,] GetInfluenceMap()
    {
        float[,] map = new float[_map_size.x, _map_size.y];

        MapCell[] buildings = GetAllTargetCells((x) => x.BuildingInCell != null);

        foreach (MapCell building in buildings)
        {
            foreach (MapCell neighbour in _mapManager.GetNeighboursInRadius(building.GridPosition, 2))
            {
                map[neighbour.GridPosition.x, neighbour.GridPosition.y] = 1f;
            }

        }

        return map;
    }

    private float[,] GetMineralMap()
    {

        if (_maps.TryGetValue(MapTypes.Mineral, out float[,] map)) 
            return map;

        map = new float[_map_size.x, _map_size.y];

        MapCell[] minerals = GetAllTargetCells((x) => x.Tile == MapTiles.Minerals);

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map[x, y] = EvaluateCellByDistanceToTarget(_mapManager.GetCellFromGridPosition(new Vector2Int(x, y)), minerals);
            }
        }

        return map;
    }

    private MapCell[] GetAllTargetCells(Func<MapCell, bool> targeting)
    { 
        List<MapCell> cells = new List<MapCell>();
        Vector2Int size = _mapManager.GetMapSize();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var cell = _mapManager.GetCellFromGridPosition(new Vector2Int(x, y));
                if (targeting(cell))
                    cells.Add(cell);
            }
        }
        return cells.ToArray();
    }

    private float EvaluateCellByDistanceToTarget(MapCell cell, MapCell[] target_cells)
    {
        float distance = Mathf.Infinity;
        foreach (MapCell mineral in target_cells)
        {
            distance = Mathf.Min(distance, _mapManager.GetDistance(cell, mineral));
        }
        return EvaluateByDistance(distance);
    }

    private float EvaluateByDistance(float distance)
    {
        if (_max_distance == 0) _max_distance = Mathf.Sqrt(Mathf.Pow(_map_size.x, 2) + Mathf.Pow(_map_size.y, 2));
        return distance / _max_distance;
    }

    public float[,] GetMapByType(MapTypes type)
    {
        return _maps[type];
    }

    public bool IsRectBuildable(Vector2Int upper_left_corner, Vector2Int size)
    {
        return !RectCheck(upper_left_corner, size, MapTypes.PlayerBuildings, (x) => x == 1f);
    }

    public bool IsMineralsInRect(Vector2Int upper_left_corner, Vector2Int size)
    {
        return RectCheck(upper_left_corner, size, MapTypes.Mineral, (x) => x == 0f);
    }

    // true if any cell in rect fits activation func
    private bool RectCheck(Vector2Int upper_left_corner, Vector2Int size, MapTypes layer, Func<float, bool> activation)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (AbnormalGridPosition(upper_left_corner + new Vector2Int(x, -y)))
                    continue;

                if (activation(_maps[layer][upper_left_corner.x + x, upper_left_corner.y - y]))
                    return true;
            }
        }
        return false;
    }

    private bool AbnormalGridPosition(Vector2Int pos)
    {
        return pos.x < 0 || pos.x >= _map_size.x || pos.y < 0 || pos.y >= _map_size.y;
    }

}