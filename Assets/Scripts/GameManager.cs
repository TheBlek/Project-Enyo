﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cell_size;
    [SerializeField] private int metals;
    [SerializeField] private Builder builder;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Vector2Int map_size;
    [SerializeField] private GameObject enemyPrefab;
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

        gridManager = new GridManager();
        gridManager.InitGrid(map_size, cell_size);

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
    public Vector2Int[] GetGridCellsIndexForBuilding(Vector2 pos, Vector2 size)
    {
        return gridManager.GetGridCellsIndexInRect(pos, size);
    }

    public Vector2Int GetGridCellIndexFromCoords(Vector2 coords)
    {
        return gridManager.GetGridCellIndexFromCoords(coords);
    }

    public Vector2Int[] GetCellNeighbours(Vector2Int cell)
    {
        return gridManager.GetStraightNeighbours(cell);
    }
    #endregion

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

    public Building GetRandomBuilding()
    {
        if (buildings.Count == 0)
            return null;
        return buildings[random.Next(buildings.Count)];
    }
    #endregion
}
