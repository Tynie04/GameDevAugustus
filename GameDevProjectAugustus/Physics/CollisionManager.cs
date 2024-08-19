using System;
using System.Collections.Generic;
using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Managers;

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
        sprite.GetRectangle();

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
    bool isInWater = false; // Flag to track if the player is in water

    foreach (var tile in intersectingTiles)
    {
        if (tile.Value == 3)
        {
            // Special handling for tile ID 3
            HandleSpecialTileCollision(sprite, tile.Key, horizontal);
            continue; // Skip further checks for this tile
        }

        if (sprite.Rect.Intersects(tile.Key))
        {
            // Check if the tile is water (ID 1)
            if (tile.Value == 1)
            {
                isInWater = true; // Player is in water
                Console.WriteLine("Sprite is in water.");
            }

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
                    sprite.IsGrounded = true; // Default to true
                }
                else if (sprite.Velocity.Y < 0) // Moving up
                {
                    sprite.Rect.Y = tile.Key.Bottom;
                }
                sprite.Velocity.Y = 0;
            }
        }
    }

    // Handle water damage if the player is in water
    if (isInWater)
    {
        sprite.IsInWater = true; // Set the sprite to be in water
        sprite.ApplyWaterDamage(); // Apply water damage
    }
    else
    {
        sprite.IsInWater = false; // Reset water state if not in water
    }
}

private static void HandleSpecialTileCollision(Sprite sprite, Rectangle tileRect, bool horizontal)
{
    if (horizontal)
    {
        // No special handling needed for horizontal movement for tile ID 3
        return;
    }

    // Vertical collision handling for tile ID 3
    if (sprite.Velocity.Y > 0) // Moving down
    {
        // Smoothly move sprite to the top of the tile
        sprite.Rect.Y = tileRect.Top - sprite.Rect.Height;
        sprite.Velocity.Y = 0;
        sprite.IsGrounded = true; // Default to true
    }
    else if (sprite.Velocity.Y < 0) // Moving up
    {
        sprite.Rect.Y = tileRect.Bottom;
        sprite.Velocity.Y = 0;
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