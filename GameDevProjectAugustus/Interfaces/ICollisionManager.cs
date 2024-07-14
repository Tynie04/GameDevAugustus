using GameDevProjectAugustus;

public interface ICollisionManager
{
    void HandleCollisions(Sprite sprite, Level level, int tileSize);
}