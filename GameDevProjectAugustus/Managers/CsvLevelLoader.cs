using System;
using System.Collections.Generic;
using System.IO;
using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Managers;

public class CsvLevelLoader : ILevelLoader
{
    private readonly string _basePath;

    public CsvLevelLoader(string basePath)
    {
        _basePath = basePath;
    }

    public Level LoadLevel(string levelName)
    {
        var ground = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_ground.csv"), "Ground");
        var platforms = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_platforms.csv"), "Platforms");
        var collisions = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_collisions.csv"), "Collisions");
        var props = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_props.csv"), "Props");
        var spawns = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_spawns.csv"), "Spawns");
        var water = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_water.csv"), "Water");
        var finish = LoadMap(Path.Combine(_basePath, levelName, $"{levelName}_finish.csv"), "Finish");

        return new Level(ground, platforms, collisions, props, spawns, water, finish);
    }


    private Dictionary<Vector2, int> LoadMap(string filePath, string layerName)

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
                            Vector2 position = new Vector2(x, y);
                            result[position] = value;

                            // Add debug line here
                            if (value == 4)
                            {
                                Console.WriteLine($"Tile with ID 4 loaded at position: {position} in layer: {layerName}");
                            }
                        }
                    }
                }
                y++;
            }
        }
        return result;
    }

}