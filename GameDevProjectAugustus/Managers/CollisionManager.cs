﻿using System.Collections.Generic;
using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

public class CollisionManager : ICollisionManager
{
   private static void HandleHorizontalCollisions(Sprite sprite, Level level, int tileSize)
    {
        // Check collisions with ground, platforms, and any other layer you want to collide with
        HandleLayerCollisions(sprite, level.Collisions, tileSize, horizontal: true);
    }

    private static void HandleVerticalCollisions(Sprite sprite, Level level, int tileSize)
    {
        HandleLayerCollisions(sprite, level.Collisions, tileSize, horizontal: false);
    }

    public void HandleCollisions(Sprite sprite, Level level, int tileSize)
    {
        // Save the previous rectangle position to check for movement
        Rectangle previousRect = sprite.GetRectangle();

        // Move sprite based on velocity and handle collisions
        sprite.Rect.X += (int)sprite.Velocity.X;
        HandleHorizontalCollisions(sprite, level, tileSize);

        sprite.Rect.Y += (int)sprite.Velocity.Y;
        HandleVerticalCollisions(sprite, level, tileSize);

        // Ensure the sprite's rectangle is constrained within bounds after collisions
        // Example: Clamp sprite position to level bounds...
    }


private static void HandleLayerCollisions(Sprite sprite, Dictionary<Vector2, int> layer, int tileSize, bool horizontal)
{
    List<KeyValuePair<Rectangle, int>> intersectingTiles = GetIntersectingTiles(sprite.Rect, tileSize, layer);
    foreach (var tile in intersectingTiles)
    {
        if (tile.Value == 3)
        {
            continue; // Skip the tile if its value is 3
        }

        if (sprite.Rect.Intersects(tile.Key))
        {
           
                if (tile.Value == 4)
                    continue;

                if (horizontal)
                {
                    if (sprite.Velocity.X > 0) // Moving right
                    {
                        sprite.Rect.X = tile.Key.Left - sprite.Rect.Width;
                    }
                    else if (sprite.Velocity.X < 0) // Moving left
                    {
                        sprite.Rect.X = tile.Key.Right;
                    }
                    sprite.Velocity.X = 0;
                }
                else
                {
                    if (sprite.Velocity.Y > 0) // Moving down
                    {
                        sprite.Rect.Y = tile.Key.Top - sprite.Rect.Height;
                        sprite.IsGrounded = true;
                    }
                    else if (sprite.Velocity.Y < 0) // Moving up
                    {
                        sprite.Rect.Y = tile.Key.Bottom;
                    }
                    sprite.Velocity.Y = 0;
                }
        }
    }
}


    private static List<KeyValuePair<Rectangle, int>> GetIntersectingTiles(Rectangle target, int tileSize, Dictionary<Vector2, int> layer)
    {
        List<KeyValuePair<Rectangle, int>> intersectingTiles = new List<KeyValuePair<Rectangle, int>>();
        int leftTile = target.Left / tileSize;
        int rightTile = target.Right / tileSize;
        int topTile = target.Top / tileSize;
        int bottomTile = target.Bottom / tileSize;

        for (int y = topTile; y <= bottomTile; y++)
        {
            for (int x = leftTile; x <= rightTile; x++)
            {
                Vector2 tilePosition = new Vector2(x, y);
                if (layer.ContainsKey(tilePosition))
                {
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    int tileValue = layer[tilePosition];
                    intersectingTiles.Add(new KeyValuePair<Rectangle, int>(tileRect, tileValue));
                }
            }
        }

        return intersectingTiles;
    }
}
