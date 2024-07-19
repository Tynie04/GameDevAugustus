﻿using System.Collections.Generic;
using System.IO;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

public class CsvLevelLoader : ILevelLoader
{
    private readonly string _basePath;

    public CsvLevelLoader(string basePath)
    {
        _basePath = basePath;
    }

    public Level LoadLevel(string levelName)
    {
        var ground = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_ground.csv"));
        var platforms = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_platforms.csv"));
        var collisions = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_collisions.csv"));
        var props = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_props.csv"));
        var spawns = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_spawns.csv"));
        var water = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_water.csv"));
        var finish = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_finish.csv"));

        return new Level(ground, platforms, collisions, props, spawns, water, finish);
    }

    private Dictionary<Vector2, int> LoadMap(string filePath)
    {
        Dictionary<Vector2, int> result = new Dictionary<Vector2, int>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value != -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                y++;
            }
        }
        return result;
    }
}
