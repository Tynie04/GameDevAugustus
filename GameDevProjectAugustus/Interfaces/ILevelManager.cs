using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Interfaces;

public interface ILevelManager
{
    Level CurrentLevel { get; }
    void LoadLevel(string levelName);
    Vector2 FindSpawnPosition(int id);
}