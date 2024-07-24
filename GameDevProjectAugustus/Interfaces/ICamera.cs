using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Interfaces;

public interface ICamera
{
    Vector2 Position { get; }
    void Update(Rectangle target, int levelWidth, int levelHeight, int screenWidth, int screenHeight);
}