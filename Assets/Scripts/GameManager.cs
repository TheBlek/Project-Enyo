using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cell_size;
    [SerializeField] private int metals;
    [SerializeField] private Builder builder;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Vector2Int map_size;

    private Vector2 grid_origin;
    private List<Building> buildings;
    private List<Bounds> buildings_bounds;

    private GridCell[,] grid;
    void Start()
    {
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();

        InitGrid();
    }
    #region Grid Operations
    private void InitGrid() 
    {
        grid = new GridCell[map_size.x, map_size.y];
        grid_origin = -Vector2.one * cell_size * map_size / 2;
        
        for (int i = 0; i < map_size.x; i++)
        {
            for (int j = 0; j < map_size.y; j++)
            {
                grid[i, j] = new GridCell();
            }
        }

    }

    public Vector2Int[] GetGridCellsIndexForBuilding(Vector2 pos, Vector2 size)
    {
        Vector2 left_corner_pos = pos - size * cell_size / 2;
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                cells.Add(GetGridCellIndexFromCoords(left_corner_pos + new Vector2(i,j) * cell_size));
            }
        }
        return cells.ToArray();
    }

    public Vector2Int GetGridCellIndexFromCoords(Vector2 coords)
    {
        return new Vector2Int((int)((coords.x - grid_origin.x)/cell_size), (int)((coords.y - grid_origin.y) / cell_size));
    }

    private void AdjustCellsForBuilding(Building building)
    {
        Vector2Int[] cells = GetGridCellsIndexForBuilding(building.transform.position, building.GetSize());
        foreach (Vector2Int cell in cells)
        {
            grid[cell.x, cell.y].SetBuilding(building);
        }

    }

    public Vector2Int[] GetCellNeighbours(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>
        {
            new Vector2Int(cell.x, cell.y + 1),
            new Vector2Int(cell.x + 1, cell.y),
            new Vector2Int(cell.x, cell.y - 1),
            new Vector2Int(cell.x - 1, cell.y)
        };
        return neighbours.ToArray();
    }

    #endregion

    public void BuildingInsertion(Building building)
    {
        metals -= building.GetCost();

        buildings.Add(building);
        AdjustCellsForBuilding(building);

        buildings_bounds.Add(building.GetComponent<BoxCollider2D>().bounds);

        building.SetUp(this);
    }
    
    public bool IsThereAWall(Vector3 pos)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].GetName() == "Wall" && buildings_bounds[i].Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i] == null)
            {
                buildings.Remove(buildings[i]);
                buildings_bounds.Remove(buildings_bounds[i]);
            }
            else
                buildings[i].SelfUpdate(this);
        }
    }

    #region Some Get Methods
    public Building GetBuildingInCell(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= map_size.x || cell.y < 0 || cell.y >= map_size.y)
            return null;
        return grid[cell.x, cell.y].GetBuilding();
    }

    public bool IsCellBuildable(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= map_size.x || cell.y < 0 || cell.y >= map_size.y)
            return false;
        return grid[cell.x, cell.y].Buildable();
    }
    public bool IsAffordable(Building building)
    {
        return building.GetCost() <= metals;
    }

    public int GetMetals()
    {
        return metals;
    }

    public float GetGridSize()
    {
        return cell_size;
    }

    public List<Bounds> GetBuildingsBounds()
    {
        return buildings_bounds;
    }

    public void AddMetals(int addition)
    {
        metals += addition;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.GetPlayerPosition();
    }
    #endregion
}
