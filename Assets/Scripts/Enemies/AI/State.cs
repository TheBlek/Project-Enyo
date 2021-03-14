using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEditor;


public enum MapTypes
{
    Mineral,
    Influence,
    Buildability
}

[BurstCompile]
public struct UpdateMIneralMapOldWayJob : IJobParallelFor
{
    public float _max_distance;
    public int _map_width;
    [ReadOnly]
    public NativeArray<Vector2Int> _minerals;

    public NativeArray<float> _map;
    public NativeArray<Vector2Int> _distance_source;

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
        _distance_source[index] = source;
    }
}

public class State
{

    private MapManager _mapManager;
    private Vector2Int _map_size;
    private float _max_distance;

    private Dictionary<MapTypes, float[,]> _maps;

    public Action OnStateChange;

    private Vector2Int[,] _distance_source;
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
        Stopwatch sw = Stopwatch.StartNew();
        if (_distance_source == null)
            _distance_source = new Vector2Int[_map_size.x, _map_size.y];

        MapCell[] minerals = GetAllTargetCells((x) => x.Tile == MapTiles.Minerals && x.BuildingInCell == null);

        if (!_maps.TryGetValue(MapTypes.Mineral, out float[,] map))
        {
            map = SetUpMineralMap(minerals);
            sw.Stop();
            UnityEngine.Debug.Log("Building mineral map took " + sw.ElapsedMilliseconds + " ms");

            return map;
        }

        if (minerals.Length == _old_minerals?.Length) return map;

        map = UpdateMineralMapByJobOldWay(Array.ConvertAll(minerals, (el) => (Vector2Int)el));

        _old_minerals = minerals;

        sw.Stop();
        UnityEngine.Debug.Log("Updating mineral map took " + sw.ElapsedMilliseconds + " ms");

        return map;

    }
    private float[,] SetUpMineralMap(MapCell[] minerals)
    {
        float[,] map = new float[_map_size.x, _map_size.y];
        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map[x, y] = 1f;
            }
        }

        foreach (MapCell mineral in minerals)
        {
            List<Vector2Int> temp = new List<Vector2Int>();
            int max_radius = Mathf.Max(mineral.GridPosition.x, mineral.GridPosition.y);
            int tmp = Mathf.Max(_map_size.x - mineral.GridPosition.x, _map_size.y - mineral.GridPosition.y);
            max_radius = Mathf.Max(max_radius, tmp);
            int i = 0;
            bool useful = false;
            while (i < max_radius && !useful)
            {
                MapCell[] cells = _mapManager.GetNeighboursInCircle(mineral.GridPosition, i);
                float new_value = i / _max_distance;
                useful = true;
                foreach (Vector2Int cell in cells)
                {
                    if (map[cell.x, cell.y] > new_value)
                    {
                        map[cell.x, cell.y] = new_value;
                        _distance_source[cell.x, cell.y] = mineral.GridPosition;
                        useful = false;
                        temp.Add(new Vector2Int(cell.x, cell.y));
                    }
                }
                i++;
            }
            _cells_for_concrete_mineral[mineral.GridPosition.x, mineral.GridPosition.y] = temp;
        }

        _old_minerals = minerals;
        return map;
    }

    private float[,] UpdateMineralMapCached(Vector2Int[] minerals)
    {
        float[,] map = _maps[MapTypes.Mineral];

        foreach (Vector2Int mineral in _old_minerals)
        {
            if (Array.Exists(minerals, (element) => element == mineral))
                continue;

            foreach (Vector2Int cell in _cells_for_concrete_mineral[mineral.x, mineral.y].ToArray())
            {
                int i = Mathf.RoundToInt(map[cell.x, cell.y] * _max_distance);
                bool useful = true;
                while (i < _max_distance && useful)
                {
                    MapCell[] neighbours = _mapManager.GetNeighboursInCircle(cell, i);
                    float min_value = Mathf.Infinity;
                    Vector2Int new_mineral = default;
                    foreach (Vector2Int neighbour in neighbours)
                    {
                        if (min_value > map[neighbour.x, neighbour.y] && Array.Exists(minerals, (el) => el == _distance_source[neighbour.x, neighbour.y]))
                        {
                            min_value = map[neighbour.x, neighbour.y];
                            new_mineral = _distance_source[neighbour.x, neighbour.y];
                        }
                    }
                    if (min_value != Mathf.Infinity)
                    {
                        map[cell.x, cell.y] = min_value + i / _max_distance;
                        _distance_source[cell.x, cell.y] = new_mineral;
                        _cells_for_concrete_mineral[new_mineral.x, new_mineral.y].Add(cell);
                        useful = false;
                        break;
                    }

                    i++;
                }
            }

            _cells_for_concrete_mineral[mineral.x, mineral.y].Clear();
        }

        return map;
    }

    private float[,] UpdateMineralMapNonCached(Vector2Int[] minerals)
    {
        float[,] map = _maps[MapTypes.Mineral];

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                if (Array.Exists(minerals, (el) => el == _distance_source[x, y]))
                    continue;

                int i = Mathf.RoundToInt(map[x, y] * _max_distance);
                bool useful = true;
                while (i < _max_distance && useful)
                {
                    MapCell[] neighbours = _mapManager.GetNeighboursInCircle(new Vector2Int(x, y), i);
                    float min_value = Mathf.Infinity;
                    Vector2Int supposed_mineral = default;
                    foreach (Vector2Int neighbour in neighbours)
                    {
                        if (min_value > map[neighbour.x, neighbour.y] && Array.Exists(minerals, (el) => el == _distance_source[neighbour.x, neighbour.y]))
                        {
                            min_value = map[neighbour.x, neighbour.y];
                            supposed_mineral = _distance_source[neighbour.x, neighbour.y];
                        }
                    }
                    if (min_value != Mathf.Infinity)
                    {
                        map[x, y] = min_value + i / _max_distance;
                        _distance_source[x, y] = supposed_mineral;
                        useful = false;
                        break;
                    }

                    i++;
                }
            }
        }

        return map;
    }

    private float[,] UpdateMineralMapByJob(Vector2Int[] minerals)
    {
        float[,] map = _maps[MapTypes.Mineral];

        NativeArray<float> map_array = new NativeArray<float>(_map_size.x * _map_size.y, Allocator.TempJob);
        NativeArray<Vector2Int> distance_source = new NativeArray<Vector2Int>(_map_size.x * _map_size.y, Allocator.TempJob);
        NativeArray<Vector2Int> minerals_array = new NativeArray<Vector2Int>(minerals.Length, Allocator.TempJob);

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map_array[x * _map_size.x + y] = map[x, y];
                distance_source[x * _map_size.x + y] = _distance_source[x, y];
            }
        }
        for (int i = 0; i < minerals.Length; i++)
        {
            minerals_array[i] = minerals[i];
        }

        UpdateMineralMapJob job = new UpdateMineralMapJob
        {
            _giver = new NeighbourGiver { _map_size = _map_size },
            _map_width = _map_size.x,
            _minerals = minerals_array,
            _max_distance = _max_distance,
            _map = map_array,
            _distance_source = distance_source
        };
        JobHandle handle = job.Schedule(map_array.Length, 1);
        handle.Complete();

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map[x, y] = map_array[x * _map_size.x + y];
                _distance_source[x, y] = distance_source[x * _map_size.x + y];
            }
        }
        map_array.Dispose();
        distance_source.Dispose();
        minerals_array.Dispose();

        return map;
    }

    private float[,] UpdateMineralMapByJobOldWay(Vector2Int[] minerals)
    {
        float[,] map = _maps[MapTypes.Mineral];

        NativeArray<float> map_array = new NativeArray<float>(_map_size.x * _map_size.y, Allocator.TempJob);
        NativeArray<Vector2Int> distance_source = new NativeArray<Vector2Int>(_map_size.x * _map_size.y, Allocator.TempJob);
        NativeArray<Vector2Int> minerals_array = new NativeArray<Vector2Int>(minerals.Length, Allocator.TempJob);

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map_array[x * _map_size.x + y] = map[x, y];
                distance_source[x * _map_size.x + y] = _distance_source[x, y];
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
            _distance_source = distance_source,
            _max_distance = Mathf.Sqrt(_map_size.x * _map_size.x + _map_size.y * _map_size.y)
        };
        JobHandle handle = job.Schedule(map_array.Length, 1);
        handle.Complete();

        for (int x = 0; x < _map_size.x; x++)
        {
            for (int y = 0; y < _map_size.y; y++)
            {
                map[x, y] = map_array[x * _map_size.x + y];
                _distance_source[x, y] = distance_source[x * _map_size.x + y];
            }
        }
        map_array.Dispose();
        distance_source.Dispose();
        minerals_array.Dispose();

        return map;
    }

    private struct NeighbourGiver
    {
        public Vector2Int _map_size;
        public Vector2Int[] GetNeighboursInCircle(Vector2Int center, int radius)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();

            for (int i = -radius; i <= radius; i++)
            {
                if (!AbnormalGridPosition(center + new Vector2Int(radius, i)))
                    neighbours.Add(center + new Vector2Int(radius, i));
                if (!AbnormalGridPosition(center + new Vector2Int(i, radius)))
                    neighbours.Add(center + new Vector2Int(i, radius));
                if (!AbnormalGridPosition(center + new Vector2Int(-radius, i)))
                    neighbours.Add(center + new Vector2Int(-radius, i));
                if (!AbnormalGridPosition(center + new Vector2Int(i, -radius)))
                    neighbours.Add(center + new Vector2Int(i, -radius));
            }
            return neighbours.ToArray();
        }
        private bool AbnormalGridPosition(Vector2Int position)
        {
            return position.x < 0 || position.x >= _map_size.x || position.y < 0 || position.y >= _map_size.y;
        }
    }

    private struct UpdateMineralMapJob : IJobParallelFor
    {
        public NeighbourGiver _giver;
        public int _map_width;
        public float _max_distance;

        public NativeArray<Vector2Int> _minerals;
        public NativeArray<float> _map;
        public NativeArray<Vector2Int> _distance_source; 
        public void Execute(int index)
        {
            Vector2Int mineral = _distance_source[index];
            Vector2Int[] minerals = _minerals.ToArray();
            if (Array.Exists(minerals, (el) => el == mineral))
                return;

            int x = index / _map_width;
            int y = index % _map_width;
            Vector2Int[] distance_source = _distance_source.ToArray();
            float[] map = _map.ToArray(); 

            int i = Mathf.RoundToInt(map[index] * _max_distance);
            while (i < _max_distance)
            {
                Vector2Int[] neighbours = _giver.GetNeighboursInCircle(new Vector2Int(x, y), i);
                float min_value = Mathf.Infinity;
                Vector2Int supposed_mineral = default;
                foreach (Vector2Int neighbour in neighbours)
                {
                    int neighbour_index = neighbour.x * _map_width + neighbour.y;
                    Vector2Int neighbour_mineral = distance_source[neighbour_index];
                    if (min_value > map[neighbour_index] && Array.Exists(minerals, (el) => el == neighbour_mineral))
                    {
                        min_value = map[neighbour_index];
                        supposed_mineral = distance_source[neighbour_index];
                    }
                }
                if (min_value != Mathf.Infinity)
                {
                    _map[index] = min_value + i / _max_distance;
                    _distance_source[index] = supposed_mineral;
                    return;
                }

                i++;
            }
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
        float distance = Mathf.Infinity;
        foreach (Vector2Int mineral in target_cells)
        {
            float dist = Vector2.Distance(cell_pos, mineral);
            if (dist < distance)
            {
                distance = dist;
                _distance_source[cell_pos.x, cell_pos.y] = mineral;
            }
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
        return !RectCheck(upper_left_corner, size, MapTypes.Buildability, (x) => x != 1f);
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

    public Vector2Int GetMapSize()
    {
        return _map_size;
    }

}