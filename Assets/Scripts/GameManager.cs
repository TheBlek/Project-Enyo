using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float grid;
    [SerializeField] private int metals;
    [SerializeField] private Builder builder;

    private List<Building> buildings;
    private List<Bounds> buildings_bounds;
    void Start()
    {
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();
    }

    public void BuildingInsertion(Building building)
    {
        buildings.Add(building);
        building.gameManager = this;
        metals -= building.GetCost();
        buildings_bounds.Add(building.GetComponent<BoxCollider>().bounds);
    }

    void Update()
    {
        foreach (Building building in buildings)
        {
            building.SelfUpdate();
        }
    }

    public bool IsAffordable(Building building)
    {
        if (building.GetCost() <= metals)
            return true;
        return false;
    }

    public int GetMetals()
    {
        return metals;
    }

    public float GetGridSize()
    {
        return grid;
    }

    public List<Bounds> GetBuildingsBounds()
    {
        return buildings_bounds;
    }

    public void AddMetals(int addition)
    {
        metals += addition;
    }
}
