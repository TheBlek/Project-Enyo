using UnityEngine;

public class Barrack : Building
{
    [SerializeField] private UnitsHub _unit_hub;

    private bool _in_proccess = false;
    private Unit _unit_in_production;
    private Animator _animator;

    private MapManager _mapManager;

    public delegate void OnSpawn(Enemy spawned_enemy);
    public OnSpawn onSpawn;

    private new void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _mapManager = FindObjectOfType<GameManager>().GetMapManager();
    }

    public void TrySchedule(Enemies enemy)
    {
        if (_in_proccess) return;

        _in_proccess = true;
        _unit_in_production = _unit_hub.GetUnit(enemy);
        HandleProducingAnimation();
        _maintenance_cost = _unit_in_production.ProductionCost / _unit_in_production.ProductionTime;
        Invoke(nameof(SpawnEnemy), _unit_in_production.ProductionTime);
    }

    private void HandleProducingAnimation()
    {
        _animator.speed = 1 / _unit_in_production.ProductionTime;
        //_animator.Play(_enemy_in_production.ToString());
        _animator.Play("Working");
    }

    private void SpawnEnemy()
    {
        GameObject obj = Instantiate(_unit_in_production.Prefab, PickSpawnPlace(), Quaternion.identity);
        _in_proccess = false;
        _maintenance_cost = 0;
        _animator.Play("Idle");
        onSpawn?.Invoke(obj.GetComponent<Enemy>());
    }

    private Vector2 PickSpawnPlace()
    {
        foreach (MapCell cell in _mapManager.GetCellsAroundBuilding(this))
        {
            if (cell.IsWalkable()) return _mapManager.GetGlobalPositionFromGrid(cell.GridPosition);
        }
        return default;
    }

    public bool ReadyToProduce
    {
        get
        {
            return !_in_proccess;
        }
    }
}