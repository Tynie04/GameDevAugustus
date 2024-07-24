using System;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Classes;

public class Camera : ICamera
{
    public Vector2 Position { get; private set; }

    public void Update(Rectangle target, int levelWidth, int levelHeight, int screenWidth, int screenHeight)
    {
        Position = new Vector2(
            Math.Max(0, Math.Min(target.Center.X - screenWidth / 2, levelWidth - screenWidth)),
            Math.Max(0, Math.Min(target.Center.Y - screenHeight / 2, levelHeight - screenHeight))
        );
    }
}
