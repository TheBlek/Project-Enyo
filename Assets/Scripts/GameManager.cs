using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int metals = 0;
    [SerializeField] private Builder builder;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Superviser superviser;

    private List<Building> buildings;
    private List<Bounds> buildings_bounds;
    private GridManager gridManager;
    private System.Random random;

    void Start()
    {
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();

        gridManager = GetComponent<GridManager>();

        random = new System.Random();
    }

    public void BuildingInsertion(Building building)
    {
        metals -= building.GetCost();

        buildings.Add(building);
        gridManager.AdjustCellsForBuilding(building);

        buildings_bounds.Add(building.GetComponent<BoxCollider2D>().bounds);

        building.SetUp(this);
    }
    

    private void Update()
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

    private void FixedUpdate()
    {
        superviser.SelfUpdate(this);
    }

    #region Some Get Methods
    #region Grid Getters
    public Building GetBuildingInCell(Vector2Int cell)
    {
        return gridManager.GetBuildingInCell(cell);
    }
    public bool IsCellBuildable(Vector2Int cell)
    {
        return gridManager.IsCellBuildable(cell);
    } 

    public bool IsRectBuildable(Vector2 pos, Vector2 size)
    {
        return gridManager.IsRectBuildable(pos, size);
    }

    public Vector2Int GetGridPositionFromGlobal(Vector2 global_pos)
    {
        return gridManager.GetGridPositionFromGlobal(global_pos);
    }

    public Cell[] GetCellStraightNeighbours(Vector2Int grid_position)
    {
        return gridManager.GetStraightNeighbours(grid_position);
    }
    #endregion

    public bool IsThereAWall(Vector3 pos)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].GetBuildingType() == Buildings.Wall && buildings_bounds[i].Contains(pos))
            {
                return true;
            }
        }
        return false;
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
        return gridManager.GetCellSize();
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

    public Building GetRandomBuilding()
    {
        if (buildings.Count == 0)
            return null;
        return buildings[random.Next(buildings.Count)];
    }
    #endregion
}
