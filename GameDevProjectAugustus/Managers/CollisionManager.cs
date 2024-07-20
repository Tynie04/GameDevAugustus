using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class CollisionManager : ICollisionManager
{
    public void HandleCollisions(Sprite sprite, Level level, int tileSize)
    {
        // Save the previous rectangle position to check for movement
        Rectangle previousRect = sprite.GetRectangle();

        // Move sprite based on velocity and handle collisions
        sprite.rect.X += (int)sprite.velocity.X;
        HandleHorizontalCollisions(sprite, level, tileSize);

        sprite.rect.Y += (int)sprite.velocity.Y;
        HandleVerticalCollisions(sprite, level, tileSize);

        // Ensure the sprite's rectangle is constrained within bounds after collisions
        // Example: Clamp sprite position to level bounds if needed
    }

    private static void HandleHorizontalCollisions(Sprite sprite, Level level, int tileSize)
    {
        // Check collisions with ground, platforms, and any other layer you want to collide with
        HandleLayerCollisions(sprite, level.Collisions, tileSize, horizontal: true);
    }

    private static void HandleVerticalCollisions(Sprite sprite, Level level, int tileSize)
    {
        HandleLayerCollisions(sprite, level.Collisions, tileSize, horizontal: false);
    }

    private static void HandleLayerCollisions(Sprite sprite, Dictionary<Vector2, int> layer, int tileSize, bool horizontal)
    {
        List<KeyValuePair<Rectangle, int>> intersectingTiles = GetIntersectingTiles(sprite.rect, tileSize, layer);
        foreach (var tile in intersectingTiles)
        {
            if (tile.Value == 3)
            {
                continue; // Skip the tile if its value is 3
            }

            if (sprite.rect.Intersects(tile.Key))
            {
                if (horizontal)
                {
                    if (sprite.velocity.X > 0) // Moving right
                    {
                        sprite.rect.X = tile.Key.Left - sprite.rect.Width;
                    }
                    else if (sprite.velocity.X < 0) // Moving left
                    {
                        sprite.rect.X = tile.Key.Right;
                    }
                    sprite.velocity.X = 0;
                }
                else
                {
                    if (sprite.velocity.Y > 0) // Moving down
                    {
                        sprite.rect.Y = tile.Key.Top - sprite.rect.Height;
                        sprite.isGrounded = true;
                    }
                    else if (sprite.velocity.Y < 0) // Moving up
                    {
                        sprite.rect.Y = tile.Key.Bottom;
                    }
                    sprite.velocity.Y = 0;
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
