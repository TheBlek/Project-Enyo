using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Superviser : MonoBehaviour
{
    private GameManager _gameManager;
    private MapManager _mapManager;

    [SerializeField] private Gradient _gradient;
    [SerializeField] bool _show_gizmos;
    [SerializeField] MapTypes _current_type;
    [SerializeField] private Behaviour[] _behaviours;
    [SerializeField] private float _turn_delay;
    private State _state;

    private Builder _builder;

    private Behaviour _behaviour_to_follow;

    private bool _ready_to_process_instruction = true;

    private bool _is_behaviour_to_follow_exists = true;

    private void Start()
    { 
        _gameManager = FindObjectOfType<GameManager>();
        _mapManager = _gameManager.GetMapManager();
        _builder = GetComponent<Builder>();

        _state = new State(_mapManager, _builder, new Buildings[]{ Buildings.Mine, Buildings.Barrack });
        _state.OnStateChange += ReEvaluateBestBehaviour;
    }


    public void SelfUpdate()
    {
        if (!_is_behaviour_to_follow_exists) return;

        if (_behaviour_to_follow == null)
            ReEvaluateBestBehaviour();

        if (!_is_behaviour_to_follow_exists) return;

        if (!_behaviour_to_follow.IsDone)
        {
            ProcessInstruction(_behaviour_to_follow.NextInstruction());
        } else {
            _behaviour_to_follow.Reset();
            ReEvaluateBestBehaviour();
        }
    }

    private void FixedUpdate()
    {
        foreach (Enemy enemy in _state.GetUnits())
            enemy.SelfUpdate();
    }

    private void ReEvaluateBestBehaviour()
    {
        _behaviour_to_follow = PickBestAvailableBehaviour();
        _is_behaviour_to_follow_exists = _behaviour_to_follow != null;
    }

    private Behaviour PickBestAvailableBehaviour()
    {
        Behaviour best = null;
        foreach (Behaviour behaviour in _behaviours)
            if (behaviour.Trigger(_state) && ( best == null || best.Value < behaviour.Value))
                best = behaviour;
        return best;
    }

    private void ProcessInstruction(Instruction instruction)
    {
        if (!_ready_to_process_instruction)
            return;
        _ready_to_process_instruction = false;
        switch (instruction.Type)
        {
            case InstructionTypes.Build:
                Build(instruction.Parameters);
                break;

            case InstructionTypes.BuildUnit:
                ProduceUnit(instruction.Parameters);
                break;
            case InstructionTypes.MoveUnit:
                MoveUnit(instruction.Parameters);
                break;

            default:
                break;
        }
        Invoke(nameof(ResetReadyness), _turn_delay);
    }

    private void MoveUnit(object[] parameters)
    {
        Enemy unit = (Enemy)parameters[0];
        Vector2 pos = _mapManager.GetGlobalPositionFromGrid((Vector2Int)parameters[1]);
        //Debug.Log("yeah, I'm setting him at pos: " + pos);
        unit.Target = pos;
        unit.SetBehaviourPatter(BehaviourPattern.Path);
    }

    private void ResetReadyness()
    {
        _ready_to_process_instruction = true;
    }

    private void Build(object[] parameters)
    { 
        _builder.SetBuildingType((Buildings)parameters[1]);

        Vector2 global_pos = _mapManager.GetGlobalPositionFromGrid((Vector2Int)parameters[0]);
        _builder.Build(_gameManager, _builder.StickPositionToGrid(global_pos, _mapManager.GetCellSize()));
    }

    private void ProduceUnit(object[] parameters)
    {
        Barrack b = (Barrack)parameters[0];
        b.TrySchedule((Enemies)parameters[1]);
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
