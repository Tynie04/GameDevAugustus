using System.Collections.Generic;
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
        var ground = LoadMap(Path.Combine(_basePath, $"{levelName}_ground.csv"));
        var platforms = LoadMap(Path.Combine(_basePath, $"{levelName}_platforms.csv"));
        var collisions = LoadMap(Path.Combine(_basePath, $"{levelName}_collisions.csv"));
        
        return new Level(ground, platforms, collisions);
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