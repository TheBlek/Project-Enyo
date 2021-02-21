using System;
using UnityEngine;

public class NeighbourAdjuster<T, K> where K : ISimilarity<K>
{
    private T[] _pattern_results;
    private int pattern;

    public NeighbourAdjuster(T[] pattern_results)
    {
        if (pattern_results.Length != 16)
        {
            Debug.LogError("There aren't presented 16 patterns");
            return;
        }
        _pattern_results = pattern_results;
    }

    public void GeneratePattern(K subject, K[] neighbours)
    {
        pattern = 0;
        if (neighbours.Length != 4)
            return;
        for (int i = 0; i < 4; i++)
        {
            if (neighbours[i] != null && neighbours[i].IsSimilar(subject))
                pattern += (int)Mathf.Pow(2f, i);
        }
    }

    public T GetCurrentPattern() => _pattern_results[pattern];

    public T GetResultForSubject(K subject, K[] neighbours)
    {
        GeneratePattern(subject, neighbours);
        return GetCurrentPattern();
    }
}
