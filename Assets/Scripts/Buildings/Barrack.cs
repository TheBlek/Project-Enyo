using UnityEngine;

public class Barrack : Building
{
    [SerializeField] private GameObject[] _enemies_prefabs;
    [SerializeField] private float[] _enemies_produce_time;
    [SerializeField] private float[] _enemies_produce_cost;

    private bool _in_proccess = false;
    private Enemies _enemy_in_production;
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
        _enemy_in_production = enemy;
        HandleProducingAnimation();
        _maintenance_cost = _enemies_produce_cost[(int)enemy] / _enemies_produce_time[(int)enemy];
        Invoke(nameof(SpawnEnemy), _enemies_produce_time[(int)enemy]);
    }

    private void HandleProducingAnimation()
    {
        _animator.speed = 1 / _enemies_produce_time[(int) _enemy_in_production];
        //_animator.Play(_enemy_in_production.ToString());
        _animator.Play("Working");
    }

    private void SpawnEnemy()
    {
        GameObject obj = Instantiate(_enemies_prefabs[(int)_enemy_in_production], PickSpawnPlace(), Quaternion.identity);
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