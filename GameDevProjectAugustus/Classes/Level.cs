using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Level
{
    public Dictionary<Vector2, int> Ground { get; }
    public Dictionary<Vector2, int> Platforms { get; }
    public Dictionary<Vector2, int> Collisions { get; }

    public Level(Dictionary<Vector2, int> ground, Dictionary<Vector2, int> platforms, Dictionary<Vector2, int> collisions)
    {
        Ground = ground;
        Platforms = platforms;
        Collisions = collisions;
    }
}