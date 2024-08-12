using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Managers;

public class LevelManager : ILevelManager
{
    private readonly ILevelLoader _levelLoader;
    private Level _currentLevel;

    public LevelManager(ILevelLoader levelLoader)
    {
        _levelLoader = levelLoader;
    }

    public Level CurrentLevel => _currentLevel;

    public void LoadLevel(string levelName)
    {
        _currentLevel = _levelLoader.LoadLevel(levelName);
    }

    public Vector2 FindSpawnPosition(int id)
    {
        foreach (var kvp in _currentLevel.Spawns)
        {
            if (kvp.Value == id)
            {
                return kvp.Key;
            }
        }
        return Vector2.Zero;
    }
}