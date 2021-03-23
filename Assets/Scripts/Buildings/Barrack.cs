using UnityEngine;

public class Barrack : Building
{
    [SerializeField] private GameObject[] _enemies_prefabs;
    [SerializeField] private float[] _enemies_produce_time;

    private bool _in_proccess = false;
    private Enemies _enemy_in_production;
    private Animator _animator;

    private MapManager _mapManager;

    private new void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
    }

    public void TrySchedule(Enemies enemy)
    {
        if (_in_proccess) return;

        _in_proccess = true;
        _enemy_in_production = enemy;
        HandleProducingAnimation();
        Invoke(nameof(SpawnEnemy), _enemies_produce_time[(int)enemy]);
    }

    private void HandleProducingAnimation()
    {
        _animator.speed = 1 / _enemies_produce_time[(int) _enemy_in_production];
        _animator.Play(_enemy_in_production.ToString());
    }

    private void SpawnEnemy()
    {
        Instantiate(_enemies_prefabs[(int)_enemy_in_production], PickSpawnPlace(), Quaternion.identity);
        _in_proccess = false;
    }

    private Vector2 PickSpawnPlace()
    {
        foreach (MapCell cell in _mapManager.GetCellsAroundBuilding(this))
        {
            if (cell.IsWalkable()) return cell.GridPosition;
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