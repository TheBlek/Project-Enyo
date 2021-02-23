using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/MapGenerator")]
public class MapGenerator : ScriptableObject
{

    [Header("Map generating settings")]
    [SerializeField] private float start_frequency;
    [Range(1, 10)]
    [SerializeField] private float lacunarity;
    [Range(0, 1)]
    [SerializeField] private float persistance;
    [SerializeField] private int octaves_count;
    [HideInInspector] public float[] _thresholds;

    public MapTiles[,] GenerateMap(Vector2Int map_size)
    {
        var sample = GenerateMultiLayerSample(map_size);

        var amplitude = (Mathf.Pow(persistance, octaves_count) - 1) / (persistance - 1);

        var map = ConvertSampleToMap(sample, amplitude);

        return map;
    }

    private float[,] GenerateMultiLayerSample(Vector2Int map_size)
    {
        List<float[,]> samples = new List<float[,]>();
        for (int i = 0; i < octaves_count; i++)
        {
            samples.Add(GenerateSample(map_size, start_frequency * Mathf.Pow(lacunarity, i), Mathf.Pow(persistance, i)));
        }

        var sample = CollapseLayersToOne(samples, map_size);

        return sample;
    }

    private MapTiles[,] ConvertSampleToMap(float [,] sample, float amplitude)
    {

        MapTiles[,] map = new MapTiles[sample.GetLength(0), sample.GetLength(1)];

        float min_value = amplitude;
        float max_value = 0;

        for (int x = 0; x < sample.GetLength(0); x++)
        {
            for (int y = 0; y < sample.GetLength(1); y++)
            {
                max_value = Mathf.Max(max_value, sample[x, y]);
                min_value = Mathf.Min(min_value, sample[x, y]);
                map[x, y] = ActivationFuction(sample[x, y], amplitude);
            }
        }

        //Debug.Log("min value in sample " + min_value/amplitude + " max value in sample " + max_value/amplitude);

        return map;
    }

    private float[,] CollapseLayersToOne(List<float[,]> samples, Vector2Int map_size)
    {
        float[,] final_sample = new float[map_size.x, map_size.y];

        foreach (float [,] sample in samples)
        {
            for (int x = 0; x < map_size.x; x++)
            {
                for (int y = 0; y < map_size.y; y++)
                {
                    final_sample[x, y] += sample[x, y];
                }
            }
        }
        return final_sample;
    }

    private float[,] GenerateSample(Vector2Int map_size, float frequency, float amplitude)
    {
        float[,] sample = new float[map_size.x, map_size.y];

        float offsetX = UnityEngine.Random.Range(0, 999999f);
        float offsetY = UnityEngine.Random.Range(0, 999999f);

        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                float sampleX = (float)x / map_size.x * frequency + offsetX;
                float sampleY = (float)y / map_size.y * frequency + offsetY;
                sample[x, y] = amplitude * Mathf.PerlinNoise(sampleX, sampleY);
                //Debug.Log(sampleX + " " + sampleY + " gave " + sample[x, y]);
            }
        }

        return sample;
    }

    private MapTiles ActivationFuction(float value, float amplitude)
    {
        int tile_count = Enum.GetNames(typeof(MapTiles)).Length;

        value /= amplitude; // mapping value to range 0 to 1;
        if (value < 0f)
            value = 0f; 
        if (value > 1f) 
            value = 1f;
        
        for (int i = tile_count - 1; i >= 0; i--)
        {
            if (value > _thresholds[i])
                return (MapTiles)(i);
        }
        Debug.LogWarning("Activation Function returned defaul value");
        return (MapTiles) value;
    }

    public void ApplyThresholds(float[] thresholds)
    {
        _thresholds = thresholds;
    }

}