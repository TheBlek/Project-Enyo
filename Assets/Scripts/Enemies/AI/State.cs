using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;


public enum MapTypes
{
    Mineral,
    Influence,
    Buildability
}

public class State
{

    private MapManager _mapManager;
    private Vector2Int _map_size;
    private float _max_distance;

    private Dictionary<MapTypes, float[,]> _maps;

    public Action OnStateChange;

    private MapCell[] _old_minerals;
    private List<Vector2Int>[,] _cells_for_concrete_mineral;

    public State(MapManager mapManager)
    {
        _mapManager = mapManager;
        _mapManager.OnGridChange += UpdateMaps;
        _map_size = _mapManager.GetMapSize();
        _maps = new Dictionary<MapTypes, float[,]>();
        _max_distance = Mathf.Max(_map_size.x, _map_size.y);

        _cells_for_concrete_mineral = new List<Vector2Int>[_map_size.x, _map_size.y];
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
                case MapTypes.Buildability: _maps[type] = GetBuildabilityMap(); break;
                default: break;
            }
        }
        OnStateChange?.Invoke();
    }

    private float[,] GetBuildabilityMap()
    {
        float[,] map = new float[_map_size.x, _map_size.y];

        MapCell[] buildings = GetAllTargetCells((x) => x.Buildable());

        foreach (MapCell building in buildings)
        {
            map[building.GridPosition.x, building.GridPosition.y] = 1f;
        }
        return map;
    }

    private float[,] GetInfluenceMap()
    {
        float[,] map = new float[_map_size.x, _map_size.y];

        MapCell[] buildings = GetAllTargetCells((x) => x.BuildingInCell != null && x.BuildingInCell.IsEnemy);

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
        Stopwatch sw = Stopwatch.StartNew();

        MapCell[] minerals = GetAllTargetCells((x) => x.Tile == MapTiles.Minerals && x.BuildingInCell == null);

        _maps.TryGetValue(MapTypes.Mineral, out float[,] map);

        if (minerals.Length == _old_minerals?.Length) return map;

        map = UpdateMineralMap(Array.ConvertAll(minerals, (el) => (Vector2Int)el));

        _old_minerals = minerals;

        sw.Stop();
        //UnityEngine.Debug.Log("Updating mineral map took " + sw.ElapsedMilliseconds + " ms");

        return map;

    }

    private float[,] UpdateMineralMap(Vector2Int[] minerals)
    {
        
        if (!_maps.TryGetValue(MapTypes.Mineral, out float[,] map))
        {
            map = new float[_map_size.x, _map_size.y];
        }

        NativeArray<float> map_array = new NativeArray<float>(_map_size.x * _map_size.y, Allocator.TempJob);
        NativeArray<Vector2Int> minerals_array = new NativeArray<Vector2Int>(minerals.Length, Allocator.TempJob);

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map_array[x * _map_size.x + y] = map[x, y];
            }
        }
        for (int i = 0; i < minerals.Length; i++)
        {
            minerals_array[i] = minerals[i];
        }

        UpdateMIneralMapOldWayJob job = new UpdateMIneralMapOldWayJob
        {
            _map_width = _map_size.x,
            _minerals = minerals_array,
            _map = map_array,
            _max_distance = Mathf.Sqrt(_map_size.x * _map_size.x + _map_size.y * _map_size.y)
        };
        JobHandle handle = job.Schedule(map_array.Length, 1);
        handle.Complete();

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map[x, y] = map_array[x * _map_size.x + y];
            }
        }
        map_array.Dispose();
        minerals_array.Dispose();

        return map;
    }

    [BurstCompile]
    public struct UpdateMIneralMapOldWayJob : IJobParallelFor
    {
        public float _max_distance;
        public int _map_width;
        [ReadOnly]
        public NativeArray<Vector2Int> _minerals;

        public NativeArray<float> _map;
        //public NativeArray<Vector2Int> _distance_source;

        public void Execute(int index)
        {
            int x = index / _map_width;
            int y = index % _map_width;
            float distance = Mathf.Infinity;
            Vector2Int source = default;
            for (int i = 0; i < _minerals.Length; i++)
            {
                float dist = Vector2.Distance(new Vector2Int(x, y), _minerals[i]);
                if (dist < distance)
                {
                    distance = dist;
                    source = _minerals[i];
                }
            }
            _map[index] = distance / _max_distance;
            //_distance_source[index] = source;
        }
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

    private float EvaluateCellByDistanceToTarget(Vector2Int cell_pos, Vector2Int[] target_cells)
    {
        float smallest = Mathf.Infinity;
        foreach (Vector2Int mineral in target_cells)
        {
            float distance = Vector2.Distance(cell_pos, mineral);
            if (distance < smallest)
                smallest = distance;
        }
        return EvaluateByDistance(smallest);
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

    public bool IsRectBuildable(RectInt rect)
    {
        return !RectCheck(rect.position, rect.size, MapTypes.Buildability, (x) => x != 1f);
    }

    public bool IsMineralsInRect(RectInt rect)
    {
        return RectCheck(rect.position, rect.size, MapTypes.Mineral, (x) => x == 0f);
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

    public int GetNumberOfMinerals(RectInt rect)
    {
        return NumberOfMatchesInRect(rect.position, rect.size, MapTypes.Mineral, (x) => x == 0f);
    }

    private int NumberOfMatchesInRect(Vector2Int upper_left_corner, Vector2Int size, MapTypes layer, Func<float, bool> activation)
    {
        int result = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (AbnormalGridPosition(upper_left_corner + new Vector2Int(x, -y)))
                    continue;

                if (activation(_maps[layer][upper_left_corner.x + x, upper_left_corner.y - y]))
                    result++;
            }
        }
        return result;
    }

    private bool AbnormalGridPosition(Vector2Int pos)
    {
        return pos.x < 0 || pos.x >= _map_size.x || pos.y < 0 || pos.y >= _map_size.y;
    }

    public Vector2Int GetMapSize()
    {
        return _map_size;
    }

}