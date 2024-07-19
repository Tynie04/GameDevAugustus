using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Level
{
    public Dictionary<Vector2, int> Ground { get; }
    public Dictionary<Vector2, int> Platforms { get; }
    public Dictionary<Vector2, int> Collisions { get; }
    public Dictionary<Vector2, int> Props { get; }
    public Dictionary<Vector2, int> Spawns { get; }
    public Dictionary<Vector2, int> Water { get; }
    public Dictionary<Vector2, int> Finish { get; }

    public Level(
        Dictionary<Vector2, int> ground,
        Dictionary<Vector2, int> platforms,
        Dictionary<Vector2, int> collisions,
        Dictionary<Vector2, int> props,
        Dictionary<Vector2, int> spawns,
        Dictionary<Vector2, int> water,
        Dictionary<Vector2, int> finish)
    {
        Ground = ground;
        Platforms = platforms;
        Collisions = collisions;
        Props = props;
        Spawns = spawns;
        Water = water;
        Finish = finish;
    }
}


