using System;
using UnityEngine;
using System.Collections;

public static class MapGenerator
{

    public static MapTiles[,] GenerateMap(Vector2Int map_size)
    {
        MapTiles[,] map = new MapTiles[map_size.x, map_size.y];

        var sample = GenerateSample(map_size, 20);

        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                map[x, y] = ActivationFuction(sample[x, y]);
            }
        }

        return map;
    }

    private static float[,] GenerateSample(Vector2Int map_size, float scale)
    {
        float[,] sample = new float[map_size.x, map_size.y];

        float offsetX = UnityEngine.Random.Range(0, 999999f);
        float offsetY = UnityEngine.Random.Range(0, 999999f);

        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                float sampleX = (float)x / map_size.x * scale + offsetX;
                float sampleY = (float)y / map_size.y * scale + offsetY;
                sample[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
                //Debug.Log(sampleX + " " + sampleY + " gave " + sample[x, y]);
            }
        }

        return sample;
    }

    private static MapTiles ActivationFuction(float value)
    {
        int tile_count = Enum.GetNames(typeof(MapTiles)).Length;
        
        value *= tile_count;

        float round_value = Mathf.Round(value) - 0.5f;

        Debug.Log(value + " resulted in " + (MapTiles)round_value);

        return (MapTiles) round_value;
    }

}