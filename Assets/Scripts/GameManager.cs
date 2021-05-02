using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _player_metals = 0;
    [SerializeField] private int _enemy_metals = 0;
    [SerializeField] private PlayerControls player;
    [SerializeField] private Superviser superviser;

    private List<Building> buildings;
    private MapManager mapManager;
    private PathRequestManager pathRequestManager;
    private System.Random _random;
    private float _time_since_last_maintenance = 0;

    void Awake()
    {
        SetUpBuilders();
        buildings = new List<Building>();

        mapManager = GetComponent<MapManager>();
        pathRequestManager = GetComponent<PathRequestManager>();
        PauseMenuManager = GetComponent<PauseMenuManager>();

        _random = new System.Random();
    }

    private void SetUpBuilders()
    {
        Builder[] builderers = FindObjectsOfType<Builder>();
        foreach (Builder builder in builderers)
        {
            builder.onBuild += InsertBuilding;
        }
    }

    private void InsertBuilding(Building building)
    {
        if (building.IsEnemy)
            _enemy_metals -= building.GetCost();
        else
            _player_metals -= building.GetCost();

        buildings.Add(building);
        mapManager.AdjustCellsForBuilding(building);

        building.SetUp();
    }
    
    private void Maintenance()
    {
        foreach (Building building in buildings)
            AddMetals((int)-building.MaintenanceCost, building.IsEnemy);
    }

    private void RemoveNullBuildings()
    {
        List<Building> buildings_to_remove = new List<Building>();
        foreach (Building building in buildings)
        {
            if (building == null)
            {
                buildings_to_remove.Add(building);
            }
            else
            {
                building.SelfUpdate();
            }
        }
        foreach (Building building in buildings_to_remove)
        {
            buildings.Remove(building);
        }
    }

    private void Update()
    {
        _time_since_last_maintenance += Time.deltaTime;
        RemoveNullBuildings();
        if (_time_since_last_maintenance > 1)
        {
            _time_since_last_maintenance = 0;
            Maintenance();
        }

        superviser.SelfUpdate();
    }

    private void FixedUpdate()
    {
        
    }

    #region Some Get Methods
    public MapManager GetMapManager() => mapManager;

    public PathRequestManager GetPathRequestManager() => pathRequestManager;

    public PauseMenuManager PauseMenuManager { get; private set; }

    public bool IsAffordable(Building building, bool is_enemy)
    {
        if (is_enemy)
            return building.GetCost() <= _enemy_metals;
        return building.GetCost() <= _player_metals;
    }

    public void AddMetals(int addition, bool is_enemy)
    {
        if (is_enemy)
            _enemy_metals += addition;
        else
            _player_metals += addition;
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

    public float GetMetalCount() => _player_metals;
    #endregion
}
