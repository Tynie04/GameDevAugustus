using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Classes
{
    public class Level
    {
        public Dictionary<Vector2, int> Ground { get; }
        public Dictionary<Vector2, int> Platforms { get; }
        public Dictionary<Vector2, int> Collisions { get; }
        public Dictionary<Vector2, int> Props { get; }
        public Dictionary<Vector2, int> Spawns { get; }
        public Dictionary<Vector2, int> Water { get; }
        public Dictionary<Vector2, int> Finish { get; }
        public int Width { get; }
        public int Height { get; }

        public Level(
            Dictionary<Vector2, int> ground,
            Dictionary<Vector2, int> platforms,
            Dictionary<Vector2, int> collisions,
            Dictionary<Vector2, int> props,
            Dictionary<Vector2, int> spawns,
            Dictionary<Vector2, int> water,
            Dictionary<Vector2, int> finish)
        {
            Ground = ground ?? new Dictionary<Vector2, int>();
            Platforms = platforms ?? new Dictionary<Vector2, int>();
            Collisions = collisions ?? new Dictionary<Vector2, int>();
            Props = props ?? new Dictionary<Vector2, int>();
            Spawns = spawns ?? new Dictionary<Vector2, int>();
            Water = water ?? new Dictionary<Vector2, int>();
            Finish = finish ?? new Dictionary<Vector2, int>();

            // Calculate level width and height based on the largest X and Y values in the ground tiles
            Width = Ground.Any() ? (int)(Ground.Keys.Max(v => v.X) + 1) : 0;
            Height = Ground.Any() ? (int)(Ground.Keys.Max(v => v.Y) + 1) : 0;
        }
    }
}