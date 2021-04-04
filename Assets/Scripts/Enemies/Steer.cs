using System;
using UnityEngine;
using System.Linq;

public struct Steer
{
    private readonly float[] _interests;
    private readonly float[] _dangers;
    private readonly Vector2[] _directions;
    private bool _interests_normalized;

    public Steer(Vector2[] directions)
    {
        _directions = new Vector2[directions.Length];
        for (int i = 0; i < _directions.Length; i++)
            _directions[i] = directions[i].normalized;

        _interests = new float[_directions.Length];
        _dangers = new float[_directions.Length];
        _interests_normalized = false;
    }

    public Steer(int direction_count)
    {
        _directions = new Vector2[direction_count];

        _interests = new float[_directions.Length];
        _dangers = new float[_directions.Length];
        _interests_normalized = false;

        PopulateDirections();
    }

    private void PopulateDirections()
    {
        float angle = 0;
        float offset = 2 * Mathf.PI / _directions.Length;
        for (int i = 0; i < _directions.Length; i++, angle += offset)
            _directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void AddInterestToVector(Vector2 intresting, float weight)
    {
        intresting = intresting.normalized;
        AddInterest((direction) => Vector2.Dot(direction, intresting) + 1, weight);
    }

    public void AddInterestToVectorWithDesirableDot(Vector2 intresting, float weight, float desirable_dot)
    {
        intresting = intresting.normalized;
        AddInterest((direction) => 1 - Mathf.Abs(Vector2.Dot(direction, intresting) - desirable_dot), weight);
    }

    private void AddInterest(Func<Vector2, float> function, float weight)
    {
        for (int i = 0; i < _interests.Length; i++)
            _interests[i] = Mathf.Max(function(_directions[i]) * weight, _interests[i]);
    }

    public void AddDangerToVector(Vector2 danger, float weight)
    {
        danger = danger.normalized;
        AddDanger((direction) => Vector2.Dot(direction, danger), weight);
    }

    public void AddDangerToVectorWithThreshold(Vector2 danger, float weight, float threshold)
    {
        danger = danger.normalized;
        AddDanger((direction) => Vector2.Dot(direction, danger) * weight < threshold ? 0 : Vector2.Dot(direction, danger), weight);
    }

    private void AddDanger(Func<Vector2, float> function, float weight)
    {
        for (int i = 0; i < _dangers.Length; i++)
            _dangers[i] = Mathf.Max(_dangers[i], function(_directions[i]) * weight);
    }

    public void NormalizeInterests()
    {
        float max_interest = 0;
        for (int i = 0; i < _interests.Length; i++)
            max_interest = Mathf.Max(max_interest, _interests[i]);

        if (max_interest == 0)
        {
            Debug.LogWarning("You tried to call normalization with empty interests");
            return;
        }

        for (int i = 0; i < _interests.Length; i++)
            _interests[i] /= max_interest;

        _interests_normalized = true;
    }

    public void NormalizeDangers()
    {
        float max_danger = 0;
        for (int i = 0; i < _dangers.Length; i++)
            max_danger = Mathf.Max(max_danger, _dangers[i]);

        if (max_danger == 0)
        {
            Debug.LogWarning("You tried to call normalization with empty dangers");
            return;
        }

        for (int i = 0; i < _dangers.Length; i++)
            _dangers[i] /= max_danger;

        //_interests_normalized = true;
    }

    public void ResetSteerData()
    {
        for (int i = 0; i < _interests.Length; i++)
            _interests[i] = 0;
        for (int i = 0; i < _dangers.Length; i++)
            _dangers[i] = 0;
        _interests_normalized = false;
    }

    public void VisualizeInterests(Vector2 position)
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < _directions.Length; i++)
            //if (_dangers[i] <= 0)
                Gizmos.DrawLine(position, position + _directions[i] * _interests[i]);
    }

    public void VisualizeDangers(Vector2 position)
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < _directions.Length; i++)
            Gizmos.DrawLine(position, position + _directions[i] * _dangers[i]);
    }

    public void VisualizePickedDirection(Vector2 position)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + GetTheBestDirection());
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position, position + GetResultDirection());
    }

    public Vector2 GetTheBestDirection()
    {
        if (!_interests_normalized)
            Debug.LogWarning("Be careful, interests aren't normalized");

        float minimal_danger = _dangers.Min();

        int max_index = -1;
        float max_interest = Mathf.NegativeInfinity;
        for (int i = 0; i < _interests.Length; i++)
        {
            if (_interests[i] > max_interest && _dangers[i] == minimal_danger)
            {
                max_interest = _interests[i];
                max_index = i;
            }
        }

        if (max_index == -1) return Vector2.zero;
        return _directions[max_index];
    }

    public Vector2 GetResultDirection()
    {
        Vector2 result = Vector2.zero;
        for (int i = 0; i < _interests.Length; i++)
            if (_dangers[i] == 0)
                result += _directions[i] * _interests[i];

        return result.normalized;
    }

    public Vector2[] GetDirections() => _directions;
}
