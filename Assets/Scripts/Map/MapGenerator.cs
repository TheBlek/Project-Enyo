using System;
using UnityEngine;
using System.Collections.Generic;

public static class MapGenerator
{

    public static MapTiles[,] GenerateMap(Vector2Int map_size, int octaves_count, float lacunarity, float persistance)
    {
        var sample = GenerateMultiLayerSample(map_size, octaves_count, lacunarity, persistance);

        var amplitude = (Mathf.Pow(persistance, octaves_count) - 1) / (persistance - 1);

        var map = ConvertSampleToMap(sample, amplitude);

        return map;
    }

    private static float[,] GenerateMultiLayerSample(Vector2Int map_size, int octaves_count, float lacunarity, float persistance)
    {
        List<float[,]> samples = new List<float[,]>();
        for (int i = 0; i < octaves_count; i++)
        {
            samples.Add(GenerateSample(map_size, Mathf.Pow(lacunarity, i), Mathf.Pow(persistance, i)));
        }

        var sample = CollapseLayersToOne(samples, map_size);

        return sample;
    }

    private static MapTiles[,] ConvertSampleToMap(float [,] sample, float amplitude)
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

        Debug.Log(min_value + " " + max_value);

        return map;
    }

    private static float[,] CollapseLayersToOne(List<float[,]> samples, Vector2Int map_size)
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

    private static float[,] GenerateSample(Vector2Int map_size, float frequency, float amplitude)
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

    private static MapTiles ActivationFuction(float value, float amplitude)
    {
        int tile_count = Enum.GetNames(typeof(MapTiles)).Length;

        value /= amplitude; // mapping value to range 0 to 1;

        value *= tile_count;

        float round_value = Mathf.Round(value) - 0.5f;

        //Debug.Log(value + " resulted in " + (MapTiles)round_value);

        return (MapTiles) round_value;
    }

}