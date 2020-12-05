using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float grid;
    [SerializeField] private int metals;
    [SerializeField] private BuildingControls builder;

    private List<Building> buildings;
    private List<Bounds> buildings_bounds;
    void Start()
    {
        metals = 0;
        builder.onBuild += BuildingInsertion;
        buildings_bounds = new List<Bounds>();
        buildings = new List<Building>();
    }

    public void BuildingInsertion(Building building)
    {
        buildings.Add(building);
        buildings_bounds.Add(building.GetComponent<BoxCollider>().bounds);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Building building in buildings)
        {
            building.SelfUpdate();
        }
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
