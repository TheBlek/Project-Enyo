using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Superviser : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int number_of_enemies = 4;
    [SerializeField] private float spawn_point_offset = 10;
    [SerializeField] private Vector2 spawn_point;

    private List<Enemy> enemies;
    private List<Enemy> dead_enemies;
    private GameManager _gameManager;
    private MapManager _mapManager;
    private System.Random random;

    [SerializeField] private Gradient _gradient;
    [SerializeField] bool _show_gizmos;
    [SerializeField] MapTypes _current_type;
    [SerializeField] private Behaviour[] _behaviours;
    [SerializeField] private float _turn_delay;
    private State _state;

    private Builder _builder;

    private Behaviour _behaviour_to_follow;

    private bool _processing_instruction;

    private void Start()
    {
        Array.Sort(_behaviours, (x, y) => -x.Value.CompareTo(y.Value));

        random = new System.Random();

        _gameManager = FindObjectOfType<GameManager>();
        _mapManager = _gameManager.GetMapManager();

        InitEnemies();

        _state = new State(_mapManager);
        _state.OnStateChange += ReEvaluateBestBehaviour;

        _builder = GetComponent<Builder>();

    }

    private void InitEnemies()
    {
        enemies = new List<Enemy>();
        dead_enemies = new List<Enemy>();
        for (int i = 0; i < number_of_enemies; i++)
        {
            enemies.Add(SpawnNewEnemy());
        }
    }

    public void SelfUpdate()
    {
        /*Vector3 player_pos = _gameManager.GetPlayerPosition();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                dead_enemies.Add(enemy);
            else
            {
                if (enemy.GetTarget() == EnemyTargets.player || !_gameManager.IsThereAnyBuilding()) // If target is player then set player pos as target
                    enemy.SetTarget(player_pos);
                else
                {
                    if (enemy.IsTargetEleminated && _gameManager.IsThereAnyBuilding())
                    {
                        // If previous building or any target was eliminated set new building as target
                        var a = _gameManager.GetRandomBuilding();
                        enemy.SetTarget(a.transform.position);
                        enemy.IsTargetEleminated = false;
                    }
                }
                enemy.SelfUpdate();
            }
        }
        foreach (Enemy enemy in dead_enemies)
            HandleVoidEnemy(enemy);
        dead_enemies.Clear();*/

        if (_behaviour_to_follow == null)
            return;

        if (!_behaviour_to_follow.IsDone)
        {
            ProcessInstruction(_behaviour_to_follow.NextInstruction());
        }else if (_behaviour_to_follow.IsDone)
        {
            _behaviour_to_follow.Reset();
            ReEvaluateBestBehaviour();
        }
    }

    private void ReEvaluateBestBehaviour()
    {
        _behaviour_to_follow = PickBestAvailableBehaviour();
    }

    private Behaviour PickBestAvailableBehaviour()
    {
        foreach (Behaviour behaviour in _behaviours)
        {
            if (behaviour.Trigger(_state))
            {
                return behaviour;
            }
        }
        return null;
    }

    private void ProcessInstruction(Instruction instruction)
    {
        if (_processing_instruction)
            return;
        switch (instruction.Type)
        {
            case InstructionTypes.Build:
                StartCoroutine(Build(instruction.Parameters));
                break;

            default:
                break;
        }
    }

    private IEnumerator Build(object[] parameters)
    {
        _processing_instruction = true;
        yield return new WaitForSeconds(_turn_delay);
        _builder.SetBuildingType((Buildings)parameters[1]);

        _builder.Build(_gameManager, _builder.StickPositionToGrid(_mapManager.GetGlobalPositionFromGrid((Vector2Int)parameters[0]), _mapManager.GetCellSize()));
        _processing_instruction = false;
        yield return null;
    }

    private void HandleVoidEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        enemies.Add(SpawnNewEnemy());
    }

    private Enemy SpawnNewEnemy()
    {
        Vector2 relative_pos = spawn_point_offset * new Vector2((float)random.NextDouble(), (float)random.NextDouble());

        while (!_mapManager.GetCellFromGlobalPosition(relative_pos + spawn_point).IsWalkable()) // Reroll pos while it's not walkable
            relative_pos = spawn_point_offset * new Vector2((float)random.NextDouble(), (float)random.NextDouble());

        var enemyObj = Instantiate(enemyPrefab, relative_pos + spawn_point, Quaternion.identity);
        return enemyObj.GetComponent<Enemy>();
    }

    private void OnDrawGizmos()
    {
        if (!_show_gizmos || _state == null) return;
        float[,] map = _state.GetMapByType(_current_type);
        int xMax = map.GetLength(0);
        int yMax = map.GetLength(1);
        float cell_size = _mapManager.GetCellSize();
        for (int x = 0; x < xMax; x++)
        {
            for (int y = 0; y < yMax; y++)
            {
                if (map[x, y] == 0) continue;
                Gizmos.color = _gradient.Evaluate(map[x, y]);
                Gizmos.DrawCube(_mapManager.GetGlobalPositionFromGrid(x, y), Vector3.one * cell_size);
            }
        }
    }
}
