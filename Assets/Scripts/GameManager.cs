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
    private PathRequestManager pathRequestManager;
    private System.Random random;

    void Awake()
    {
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();

        gridManager = GetComponent<GridManager>();
        pathRequestManager = GetComponent<PathRequestManager>();

        random = new System.Random();
    }

    public void BuildingInsertion(Building building)
    {
        metals -= building.GetCost();

        buildings.Add(building);
        gridManager.AdjustCellsForBuilding(building);

        buildings_bounds.Add(building.GetComponent<BoxCollider2D>().bounds);

        building.SetUp();
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
                buildings[i].SelfUpdate();
        }
    }

    private void FixedUpdate()
    {
        superviser.SelfUpdate();
    }

    #region Some Get Methods
    public GridManager GetGridManager() => gridManager;

    public PathRequestManager GetPathRequestManager() => pathRequestManager;

    public bool IsAffordable(Building building)
    {
        return building.GetCost() <= metals;
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

    public bool IsThereAnyBuilding() => buildings.Count > 0;

    public float GetMetalCount() => metals;
    #endregion
}
