using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cell_size;
    [SerializeField] private int metals;
    [SerializeField] private Builder builder;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Vector2 map_size;

    private List<Building> buildings;
    private List<Bounds> buildings_bounds;

    private GridCell[,] grid;
    void Start()
    {
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();

        grid = new GridCell[(int)map_size.x, (int)map_size.y];
    }

    private Vector2Int[] GetGridCellsIndexFromBuilding(Building building)
    {

        Vector2 center_pos = building.transform.position;
        Vector2 left_corner_pos = center_pos - building.GetSize() * cell_size / 2;
        Vector2 right_corner_pos = center_pos + building.GetSize() * cell_size / 2;
        List<Vector2Int> cells = new List<Vector2Int>();
        for (float i = left_corner_pos.x; i < right_corner_pos.x; i += cell_size)
        {
            for (float j = left_corner_pos.y; j < right_corner_pos.y; j += cell_size)
            {
                cells.Add(GetGridCellIndexFromCoords(new Vector2(i, j)));
            }
        }
        return cells.ToArray();
    }

    private Vector2Int GetGridCellIndexFromCoords(Vector2 coords)
    {
        return new Vector2Int((int)coords.x, (int)coords.y);
    }

    private void AdjustCellsForBuilding(Building building)
    {

        Vector2Int[] cells = GetGridCellsIndexFromBuilding(building);
        foreach (Vector2Int cell in cells)
        {
            grid[cell.x, cell.y].SetBuilding(building);
        }

    }

    public void BuildingInsertion(Building building)
    {
        metals -= building.GetCost();

        building.SetUp(this);

        buildings.Add(building);
        buildings_bounds.Add(building.GetComponent<BoxCollider2D>().bounds);
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
}
