using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int metals = 0;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Superviser superviser;

    private List<Building> buildings;
    private MapManager mapManager;
    private PathRequestManager pathRequestManager;
    private System.Random _random;

    void Awake()
    {
        player.GetBuilder().onBuild += BuildingInsertion;
        buildings = new List<Building>();

        mapManager = GetComponent<MapManager>();
        pathRequestManager = GetComponent<PathRequestManager>();

        _random = new System.Random();
    }

    public void BuildingInsertion(Building building)
    {
        metals -= building.GetCost();

        buildings.Add(building);
        mapManager.AdjustCellsForBuilding(building);

        building.SetUp();
    }
    

    private void Update()
    {
        foreach (Building building in buildings)
        {
            if (building == null)
                buildings.Remove(building);
            else
                building.SelfUpdate();
        }
    }

    private void FixedUpdate()
    {
        superviser.SelfUpdate();
    }

    #region Some Get Methods
    public MapManager GetMapManager() => mapManager;

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
        return buildings[_random.Next(buildings.Count)];
    }

    public bool IsThereAnyBuilding() => buildings.Count > 0;

    public float GetMetalCount() => metals;
    #endregion
}
