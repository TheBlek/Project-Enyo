using UnityEngine;
using System;

public struct ContextMap 
{
    public float[] Interests { get; private set; }
    public float[] Dangers { get; private set; }
    public Vector2[] Directions { get; private set; }
    public int DirectionsCount { get; private set; }

    private float _max_weight;

    public ContextMap (Vector2[] directions, float max_weight=1)
    {
        DirectionsCount = directions.Length;
        Interests = new float[DirectionsCount];
        Dangers = new float[DirectionsCount];
        Directions = directions;
        _max_weight = max_weight;
    }

    public ContextMap (int directions_count, float max_weight=1)
    {
        DirectionsCount = directions_count;
        Interests = new float[DirectionsCount];
        Dangers = new float[DirectionsCount];
        Directions = new Vector2[DirectionsCount];
        _max_weight = max_weight;
        PopulateDirections();
    }

    private void PopulateDirections()
    {
        float angle = 0;
        float offset = 2 * Mathf.PI / Directions.Length;
        for (int i = 0; i < Directions.Length; i++, angle += offset)
            Directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void AddInterestToVector(Vector2 intresting, float weight)
    {
        intresting = intresting.normalized;
        AddInterest((direction) => Vector2.Dot(direction, intresting) + 1, weight, 2f);
    }

    public void AddInterestToVectorWithDesirableDot(Vector2 intresting, float weight, float desirable_dot)
    {
        intresting = intresting.normalized;
        float noise = Mathf.PerlinNoise(weight, weight);
        AddInterest((direction) => 
        (1 - Mathf.Abs(Vector2.Dot(direction, intresting) - desirable_dot)) * (Vector2.SignedAngle(direction, intresting) < 0 ? 1f - 0.1f : 0.1f),
        weight);
    }

    private void AddInterest(Func<Vector2, float> function, float weight, float function_max=1)
    {
        for (int i = 0; i < Interests.Length; i++)
        {
            Interests[i] = Mathf.Max(function(Directions[i]) * weight / (_max_weight * function_max), Interests[i]);
        }
    }

    public void AddDangerToVector(Vector2 danger, float weight)
    {
        danger = danger.normalized;
        AddDanger((direction) => Vector2.Dot(direction, danger), weight);
    }

    public void AddDangerToVectorWithThreshold(Vector2 danger, float weight, float threshold)
    {
        danger = danger.normalized;
        float normalized_weight = weight / _max_weight;
        AddDanger((direction) => Vector2.Dot(direction, danger) * normalized_weight < threshold ? 0 : Vector2.Dot(direction, danger), weight);
    }

    private void AddDanger(Func<Vector2, float> function, float weight)
    {
        for (int i = 0; i < Dangers.Length; i++)
            Dangers[i] = Mathf.Max(Dangers[i], function(Directions[i]) * weight / _max_weight);
    }
}