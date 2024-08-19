using GameDevProjectAugustus.Classes;

namespace GameDevProjectAugustus.Interfaces;

public interface ICollisionManager
{
    void HandleCollisions(Sprite sprite, Level level, int tileSize);
    
}